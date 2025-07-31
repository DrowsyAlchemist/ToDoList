using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace ToDoList.SignalR
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _onlineUsers = new();

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            _onlineUsers.TryAdd(userId, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) == false)
                _onlineUsers.TryRemove(userId, out _);

            await base.OnDisconnectedAsync(exception);
        }

        public bool IsUserOnline(string userId)
        {
            return _onlineUsers.ContainsKey(userId);
        }
    }
}
