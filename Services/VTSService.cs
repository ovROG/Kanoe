using Kanoe2.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Kanoe2.Services
{
    public class VTSService
    {
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

        //-----------

        private readonly Config config;
        private readonly IHubContext<Notifications, INotificationsClient> hubContext;

        private readonly ClientWebSocket webSocet = new();
        private string? socetPort;

        //-----------

        public VTSService(Config configService, IHubContext<Notifications, INotificationsClient> hub)
        {
            config = configService;
            hubContext = hub;
        }

        private async Task DiscoverAPI()
        {
            IPEndPoint discoveryPort = new(IPAddress.Any, 47779);

            UdpClient udpServer = new();
            udpServer.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpServer.Client.Bind(discoveryPort);
            UdpReceiveResult result = await udpServer.ReceiveAsync();
            string response = Encoding.UTF8.GetString(result.Buffer);

            VTSResponse<PortDiscoveryData> data;

            try
            {
                data = JsonSerializer.Deserialize<VTSResponse<PortDiscoveryData>>(response);
            }
            catch (Exception e)
            {
                Console.WriteLine("UNABLE TO DESERIALIZE VTS DISCOVERY RESPONCE");
                Console.WriteLine(e);
                return;
            }

            socetPort = data.data.port.ToString();
            return;
        }
        private async Task Connect()
        {
            var url = new Uri("ws://localhost:" + socetPort);
            await webSocet.ConnectAsync(url, CancellationToken.None);
        }
        private async Task Auth()
        {
            if (string.IsNullOrEmpty(config.GetVTSToken()))
            {
                await hubContext.Clients.All.Notify("Please Allow Plugin in VTubeStudio", MudBlazor.Severity.Warning);
                await SendRequest("AuthenticationTokenRequest", new { pluginName = "Kanoe", pluginDeveloper = "ovROG" });
                AuthenticationToken token = await GetResponce<AuthenticationToken>();
                config.SetVTSToken(token.authenticationToken);
                config.Save();
            }

            await SendRequest("AuthenticationRequest", new { pluginName = "Kanoe", pluginDeveloper = "ovROG", authenticationToken = config.GetVTSToken() });
            AuthenticationResponse auth = await GetResponce<AuthenticationResponse>();
            if (!auth.authenticated)
            {
                Console.WriteLine("UNABLE TO AUTH TO VTS"); //TODO: Handle failed auth
                return;
            }
        }
        private async Task SendRequest(string type, object? data = null)
        {
            if (webSocet.State != WebSocketState.Open)
            {
                if (type != "AuthenticationTokenRequest" && type != "AuthenticationRequest")
                {
                    await DiscoverAPI();
                    await Connect();
                    await Auth(); // Recursive
                }
                else
                {
                    throw new Exception("Unable to Auth without connection");
                }
            }

            VTSRequest<object> requestRaw = new()
            {
                apiName = "VTubeStudioPublicAPI",
                apiVersion = "1.0",
                requestID = "KanoeApp",
                messageType = type,
                data = data
            };

            string requestJson = JsonSerializer.Serialize(requestRaw);
            byte[] requestBuffer = Encoding.UTF8.GetBytes(requestJson);
            try
            {
                await webSocet.SendAsync(requestBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception e)
            {
                Console.WriteLine("UNABLE TO SEND VTS REQUEST:");
                Console.WriteLine(e);
            }
        }
        private async Task<T> GetResponce<T>()
        {
            byte[] receiveBuffer = new byte[10000];
            int offset = 0;
            int dataPerPacket = 1000;
            try
            {
                while (true)
                {
                    ArraySegment<byte> bytesReceived = new(receiveBuffer, offset, dataPerPacket);
                    WebSocketReceiveResult result = await webSocet.ReceiveAsync(bytesReceived, CancellationToken.None);
                    offset += result.Count;
                    if (result.EndOfMessage)
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("UNABLE TO RECEIVE VTS RESPONCE");
                Console.WriteLine(e);
            }

            string responce = Encoding.UTF8.GetString(receiveBuffer, 0, offset);
            Console.WriteLine($"Responce: {responce}");
            try
            {
                return JsonSerializer.Deserialize<VTSResponse<T>>(responce).data;
            }
            catch
            {
                Console.WriteLine("UNABLE TO DESERIALIZE VTS RESPONCE TO:" + typeof(T));
                Console.WriteLine(JsonSerializer.Deserialize<VTSResponse<APIError>>(responce).data);
                throw;
            }
        }

        //Public

        public async Task<List<Hotkey>> RequestHotkeysList()
        {
            await SendRequest("HotkeysInCurrentModelRequest");
            HotkeysResponce hotkeys = await GetResponce<HotkeysResponce>();
            return new List<Hotkey>(hotkeys.availableHotkeys);
        }

        public async Task SendHotkey(string id)
        {
            await SendRequest("HotkeyTriggerRequest", new { hotkeyID = id });
            await GetResponce<HotkeyTriggerResponse>();
        }
    }
}