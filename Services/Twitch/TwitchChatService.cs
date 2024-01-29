using Kanoe.Data.Models;
using Kanoe.Hubs;
using Kanoe.Shared;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Kanoe.Services.Twitch
{
    public class TwitchChatService
    {
        private readonly TwitchClient client;
        private readonly IHubContext<Chat> hubContext;
        private readonly Config config;
        private readonly ActionsService actionsService;

        private readonly TaskCompletionSource<bool> IsConnected = new();

        public TwitchChatService(IHubContext<Chat> hub, Config configService, ActionsService aService)
        {
            hubContext = hub;
            config = configService;
            actionsService = aService;
            Random rnd = new();
            ConnectionCredentials credentials = new("justinfan" + rnd.Next(100000, 999999).ToString(), "access_token");

            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            WebSocketClient customClient = new(clientOptions);

            client = new(customClient)
            {
                WillReplaceEmotes = true
            };

            client.OnMessageReceived += Client_OnMessageReceived;

            client.OnMessageCleared += Client_OnMessageCleared;

            client.OnConnected += Client_OnConnected;

            client.Initialize(credentials);

            client.Connect(); //TODO: Auto reconnect on fails
        }

        public async Task<TwitchChatService> ConnectTo(string channel)
        {
            await IsConnected.Task;
            client.JoinChannel(channel, true);
            return this;
        }

        private void Client_OnMessageReceived(object? sender, OnMessageReceivedArgs e)
        {
            bool isCommand = e.ChatMessage.Message[0] == config.GetTwitchChatPrefix();
            if (isCommand)
            {
                Dictionary<string, string> varibles = new();
                string[] command = e.ChatMessage.Message.Split(' ', 2);
                Logger.Log($"TWITCH CHAT: Trying command:{command[0]}");
                if (command.Length > 1)
                {
                    varibles["CmdMessage"] = command[1];
                    Logger.Log($"TWITCH CHAT: With data:{command[1]}");
                }
                actionsService.FireTrigger(new TwitchChatCommand() { Command = command[0][1..] }, varibles);
            }
            Data.Models.ChatMessage Message = new(
                e.ChatMessage.Id,
                e.ChatMessage.Message,
                e.ChatMessage.DisplayName,
                e.ChatMessage.Color,
                e.ChatMessage.ColorHex,
                e.ChatMessage.EmoteSet.Emotes.Select(e => new Data.Models.Emote(e.Id, e.Name)).ToList(),
                isCommand
                );
            string json = JsonSerializer.Serialize(Message);
            hubContext.Clients.Group(e.ChatMessage.Channel).SendAsync("ReceiveMessage", json);
        }

        private void Client_OnMessageCleared(object? sender, OnMessageClearedArgs e)
        {
            hubContext.Clients.Group(e.Channel).SendAsync("ClearedMessage", e.TargetMessageId);
        }

        private void Client_OnConnected(object? sender, OnConnectedArgs e)
        {
            IsConnected.SetResult(true);
        }
    }
}
