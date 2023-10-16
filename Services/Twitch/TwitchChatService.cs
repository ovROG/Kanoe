using Kanoe2.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Kanoe2.Services.Twitch
{
    public class TwitchChatService
    {
        private readonly TwitchClient client;
        private readonly IHubContext<Chat> hubContext;
        private readonly ConnectionCredentials credentials = new("justinfan555444", "access_token");

        private readonly TaskCompletionSource<bool> IsConnected = new();

        public TwitchChatService(IHubContext<Chat> hub)
        {
            hubContext = hub;

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

            Console.WriteLine("connects");

            client.Connect();
        }

        public async Task<TwitchChatService> ConnectTo(string channel)
        {
            await IsConnected.Task;
            client.JoinChannel(channel, true);
            return this;
        }

        private void Client_OnMessageReceived(object? sender, OnMessageReceivedArgs e)
        {
            Data.Models.ChatMessage Message = new(
                e.ChatMessage.Id,
                e.ChatMessage.Message,
                e.ChatMessage.DisplayName,
                e.ChatMessage.Color,
                e.ChatMessage.ColorHex,
                e.ChatMessage.EmoteSet.Emotes.Select(e => new Data.Models.Emote(e.Id, e.Name)).ToList(),
                false
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
