using Kanoe.Data.Models;
using Kanoe.Hubs;
using Kanoe.Shared;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Kanoe.Services
{
    public class VTSService : IObserver<ObservationEvent>
    {
        //TODO: Probably better to move it somewhere
        struct VTSResponse<T>
        {
            public string apiName { get; set; }
            public string apiVersion { get; set; }
            public long timestamp { get; set; }
            public string messageType { get; set; }
            public string requestID { get; set; }
            public T data { get; set; }
        }

        struct VTSRequest<T>
        {
            public string apiName { get; set; }
            public string apiVersion { get; set; }
            public string messageType { get; set; }
            public string requestID { get; set; }
            public T? data { get; set; }
        }

        struct APIError
        {
            public int errorID { get; set; }
            public string message { get; set; }
        }

        struct PortDiscoveryData
        {
            public bool active { get; set; }
            public int port { get; set; }
            public string instanceID { get; set; }
            public string windowTitle { get; set; }
        }

        struct AuthenticationToken
        {
            public string authenticationToken { get; set; }
        }

        struct AuthenticationResponse
        {
            public bool authenticated { get; set; }
            public string reason { get; set; }
        }

        public struct Hotkey
        {
            public string name { get; set; }
            public string type { get; set; }
            public string description { get; set; }
            public string file { get; set; }
            public string hotkeyID { get; set; }
            public string[] keyCombination { get; set; } // Always empty but maybe changed in future
            public int onScreenButtonID { get; set; }
        }

        struct HotkeysResponce
        {
            public bool modelLoaded { get; set; }
            public string modelName { get; set; }
            public string modelID { get; set; }
            public Hotkey[] availableHotkeys { get; set; }
        }

        struct HotkeyTriggerResponse
        {
            public string hotkeyID { get; set; }
        }

        public struct Expression
        {
            public string name { get; set; }
            public string file { get; set; }
            public bool active { get; set; }
            public bool deactivateWhenKeyIsLetGo { get; set; }
            public bool autoDeactivateAfterSeconds { get; set; }
            public double secondsRemaining { get; set; }
            public double secondsSinceLastActive { get; set; }
            public string[] usedInHotkeys { get; set; } //TODO: change to type if delailed
            public string[] parameters { get; set; } //TODO: change to type if delailed
        }

        struct ExpressionStateResponce
        {
            public bool modelLoaded { get; set; }
            public string modelName { get; set; }
            public string modelID { get; set; }
            public Expression[] expressions { get; set; }
        }

        //-----------

        private readonly Config config;
        private readonly IHubContext<Notifications, INotificationsClient> hubContext;

        private ClientWebSocket webSocket = new();
        private string? socetPort;

        private readonly SemaphoreSlim sendSemaphore = new(1, 1);

        //-----------

        public VTSService(Config configService, IHubContext<Notifications, INotificationsClient> hub, ActionsService aService)
        {
            config = configService;
            hubContext = hub;
            aService.Subscribe(this);
        }

        private async Task DiscoverAPI()
        {
            IPEndPoint discoveryPort = new(IPAddress.Any, 47779);

            UdpClient udpServer = new();
            udpServer.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            try
            {
                udpServer.Client.Bind(discoveryPort);
                UdpReceiveResult result = await udpServer.ReceiveAsync();
                string response = Encoding.UTF8.GetString(result.Buffer);
                udpServer.Close();

                VTSResponse<PortDiscoveryData> data;
                data = JsonSerializer.Deserialize<VTSResponse<PortDiscoveryData>>(response);
                socetPort = data.data.port.ToString();
            }
            catch (Exception e)
            {
                Logger.Error("UNABLE TO DISCOVER PORT");
                Logger.Error(e.Message);
                return;
            }
            return;
        }
        private async Task Connect()
        {
            if (socetPort != null)
            {
                try
                {
                    Uri url = new("ws://localhost:" + socetPort);
                    webSocket.Dispose();
                    webSocket = new();
                    await webSocket.ConnectAsync(url, CancellationToken.None);
                }
                catch (Exception e)
                {
                    throw new Exception("UNABLE TO CONNECT TO VTS PORT\n" + e.Message);
                }
            }
            else
            {
                Logger.Error("UNABLE TO CONNECT WITHOUT PORT");
            }
        }
        private async Task Auth()
        {
            if (string.IsNullOrEmpty(config.GetVTSToken()))
            {
                await hubContext.Clients.All.Notify("Please Allow Plugin in VTubeStudio", MudBlazor.Severity.Warning);
                await Send("AuthenticationTokenRequest", new { pluginName = "Kanoe", pluginDeveloper = "ovROG" });
                AuthenticationToken token = await GetResponce<AuthenticationToken>();
                config.SetVTSToken(token.authenticationToken);
                config.Save();
            }

            await Send("AuthenticationRequest", new { pluginName = "Kanoe", pluginDeveloper = "ovROG", authenticationToken = config.GetVTSToken() });
            AuthenticationResponse auth = await GetResponce<AuthenticationResponse>();
            if (!auth.authenticated)
            {
                Logger.Log("UNABLE AUTH TO VTS WITH CURRENT TOKEN REQUESTING NEW TOKEN");
                config.SetVTSToken(null);
                throw new Exception("UNABLE AUTH TO VTS");
            }
        }
        private async Task Send(string type, object? data = null)
        {
            VTSRequest<object> requestRaw = new()
            {
                apiName = "VTubeStudioPublicAPI",
                apiVersion = "1.0",
                requestID = "KanoeApp",
                messageType = type,
                data = data
            };

            string requestJson = JsonSerializer.Serialize(requestRaw);
            ArraySegment<byte> requestBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(requestJson));
            try
            {
                await webSocket.SendAsync(requestBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                Logger.Log($"VTS Send: {requestJson} ");
            }
            catch (Exception e)
            {
                Logger.Error($"UNABLE TO SEND VTS REQUEST: {requestJson}");
                Logger.Error($"Error: {e.Message}");
                throw;
            }
        }
        private async Task<T?> GetResponce<T>()
        {
            int bufferSize = 1000;
            byte[] buffer = new byte[bufferSize];
            int offset = 0;
            int free = buffer.Length;
            try
            {
                while (true)
                {
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer, offset, free), CancellationToken.None);
                    offset += result.Count;
                    free -= result.Count;

                    if (result.EndOfMessage)
                        break;

                    if (free == 0)
                    {
                        int newSize = buffer.Length + bufferSize;
                        if (newSize > 200000)
                        {
                            throw new Exception("VTS RESPONCE IS TOO BIG");
                        }
                        byte[] newBuffer = new byte[newSize];
                        Array.Copy(buffer, 0, newBuffer, 0, offset);
                        buffer = newBuffer;
                        free = buffer.Length - offset;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("UNABLE TO RECEIVE VTS RESPONCE");
                Logger.Error($"Error: {e.Message}");
            }
            string responce = Encoding.UTF8.GetString(buffer, 0, offset);
            Logger.Log($"VTS GetResponce: {responce}");
            try
            {
                return JsonSerializer.Deserialize<VTSResponse<T>>(responce).data;
            }
            catch
            {
                Logger.Error("UNABLE TO DESERIALIZE VTS RESPONCE TO:" + typeof(T));
                return default;
            }
        }
        private async Task<T?> MakeRequest<T>(string type, object? data = null)
        {
            
            if (webSocket.State != WebSocketState.Open)
            {
                Logger.Log($"VTS Socket Connecting");
                try
                {
                    await DiscoverAPI();
                    await Connect();
                    await Auth();
                }
                catch (Exception e)
                {
                    Logger.Error("UNABLE CONNECT TO VTS:" + e.Message);
                    return default;
                }
            }

            await sendSemaphore.WaitAsync();
            try
            {
                await Send(type, data);
            }
            catch
            {
                //Retry
                try
                {
                    await DiscoverAPI();
                    await Connect();
                    await Auth();
                    await Send(type, data);
                }
                catch (Exception e)
                {
                    Logger.Error("VST Send Retry failed:" + e.Message);
                    return default;
                }
            }
            T? res = await GetResponce<T>();
            sendSemaphore.Release(1);
            return res;
        }

        //Public

        public virtual void OnCompleted()
        {
        }

        public virtual void OnError(Exception error)
        {
        }

        public virtual void OnNext(ObservationEvent e)
        {
            switch (e.Event)
            {
                case VTSHotkey vtshotkey:
                    _ = SendHotkey(vtshotkey.Id);
                    break;
                case VTSExpression vtsexpression:
                    _ = SendExpression(vtsexpression.File, vtsexpression.Active);
                    break;
            }
        }

        public async Task<List<Hotkey>> RequestHotkeysList()
        {
            HotkeysResponce? hotkeys = await MakeRequest<HotkeysResponce?>("HotkeysInCurrentModelRequest");
            if (!hotkeys.HasValue)
            {
                return new List<Hotkey>();
            }
            return new List<Hotkey>(hotkeys.Value.availableHotkeys);
        }

        public async Task SendHotkey(string id)
        {
            await MakeRequest<HotkeyTriggerResponse>("HotkeyTriggerRequest", new { hotkeyID = id });
        }

        public async Task<List<Expression>> RequestExpressionList()
        {
            ExpressionStateResponce? expression = await MakeRequest<ExpressionStateResponce?>("ExpressionStateRequest");
            if (!expression.HasValue)
            {
                return new List<Expression>();
            }
            return new List<Expression>(expression.Value.expressions);
        }

        public async Task SendExpression(string file, VTSExpression.State state = VTSExpression.State.True)
        {
            bool active = true;

            ExpressionStateResponce? current = await MakeRequest<ExpressionStateResponce?>("ExpressionStateRequest", new { expressionFile = file });
            if (!current.HasValue || current.Value.expressions.Length == 0)
            {
                return;
            }

            switch (state)
            {
                case VTSExpression.State.Invert:
                    active = !current.Value.expressions[0].active;
                    break;
                case VTSExpression.State.False:
                    active = false;
                    break;
                default:
                    break;
            }

            if (active != current.Value.expressions[0].active)
            {
                await MakeRequest<dynamic>("ExpressionActivationRequest", new { expressionFile = file, active });
            }
        }
    }
}