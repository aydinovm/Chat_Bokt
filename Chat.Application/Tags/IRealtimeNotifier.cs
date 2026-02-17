namespace Chat.Application.Tags
{
    public interface IRealtimeNotifier
    {
        Task MessageSent(Guid chatId, object payload, CancellationToken ct = default);
        Task MessagesRead(Guid chatId, object payload, CancellationToken ct = default);
        Task ChatCreated(Guid chatId, object payload, CancellationToken ct = default);
        Task ChatUpdated(Guid chatId, object payload, CancellationToken ct = default);
        Task ChatReassigned(Guid chatId, object payload, CancellationToken ct = default);

        // опционально, если хочешь персональные уведомления:
        Task NotifyUser(Guid userId, string eventName, object payload, CancellationToken ct = default);
        Task NotifyDepartment(Guid departmentId, string eventName, object payload, CancellationToken ct = default);
    }
}
