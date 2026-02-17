using Chat.API.Hubs;
using Chat.Application.Tags;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Realtime
{
    public class SignalRRealtimeNotifier : IRealtimeNotifier
    {
        private readonly IHubContext<ChatHub> _hub;

        public SignalRRealtimeNotifier(IHubContext<ChatHub> hub)
        {
            _hub = hub;
        }

        public Task MessageSent(Guid chatId, object payload, CancellationToken ct = default)
            => _hub.Clients.Group($"chat:{chatId}").SendAsync("MessageSent", payload, ct);

        public Task MessagesRead(Guid chatId, object payload, CancellationToken ct = default)
            => _hub.Clients.Group($"chat:{chatId}").SendAsync("MessagesRead", payload, ct);

        public Task ChatCreated(Guid chatId, object payload, CancellationToken ct = default)
            => _hub.Clients.Group($"chat:{chatId}").SendAsync("ChatCreated", payload, ct);

        public Task ChatUpdated(Guid chatId, object payload, CancellationToken ct = default)
            => _hub.Clients.Group($"chat:{chatId}").SendAsync("ChatUpdated", payload, ct);

        public Task ChatReassigned(Guid chatId, object payload, CancellationToken ct = default)
            => _hub.Clients.Group($"chat:{chatId}").SendAsync("ChatReassigned", payload, ct);

        public Task NotifyUser(Guid userId, string eventName, object payload, CancellationToken ct = default)
            => _hub.Clients.Group($"user:{userId}").SendAsync(eventName, payload, ct);

        public Task NotifyDepartment(Guid departmentId, string eventName, object payload, CancellationToken ct = default)
            => _hub.Clients.Group($"dept:{departmentId}").SendAsync(eventName, payload, ct);
    }
}
