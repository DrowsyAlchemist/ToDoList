using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace ToDoList.SignalR
{
    [Authorize]
    public class NotificationHub: Hub
    {
        private static readonly ConcurrentDictionary<string, string> OnlineUsers = new();

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.Name);
            OnlineUsers.TryAdd(userId, Context.ConnectionId);

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            OnlineUsers.TryRemove(userId, out _);
            await base.OnDisconnectedAsync(exception);
        }

        public bool IsUserOnline(string userId)
        {
            return OnlineUsers.ContainsKey(userId);
        }

        public async Task JoinNotificationGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }
    }
}
