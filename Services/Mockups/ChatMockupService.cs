using Kanoe2.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Kanoe2.Services.Mockups
{
    public class ChatMockupService
    {
        private readonly IHubContext<Chat> hubContext;

        public ChatMockupService(IHubContext<Chat> hub)
        {
            hubContext = hub;
        }

        public void SendMessage()
        {
            string json = JsonSerializer.Serialize(Data.Mockups.GetRandomMessage());
            hubContext.Clients.Group("i@mockup").SendAsync("ReceiveMessage", json);
        }
    }
}
