using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Chat.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var deptId = Context.User?.FindFirst("departmentId")?.Value;

            if (!string.IsNullOrWhiteSpace(userId))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");

            if (!string.IsNullOrWhiteSpace(deptId))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"dept:{deptId}");

            await base.OnConnectedAsync();
        }

        public Task JoinChat(Guid chatId)
            => Groups.AddToGroupAsync(Context.ConnectionId, $"chat:{chatId}");

        public Task LeaveChat(Guid chatId)
            => Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat:{chatId}");
    }
}
