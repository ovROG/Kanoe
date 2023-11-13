﻿using Microsoft.AspNetCore.SignalR;

namespace Kanoe.Hubs
{
    public class Chat : Hub //TODO: Add type
    {
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
