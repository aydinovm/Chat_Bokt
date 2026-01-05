namespace Chat.Domain.Persistence
{
    public class ChatReassignmentHistoryModel : BaseEntity, IEntity
    {
        // Всегда есть чат
        public Guid ChatRequestId { get; set; }
        public ChatRequestModel ChatRequest { get; set; } = null!;

        // Кто переназначил
        public Guid ReassignedByUserId { get; set; }
        public UserModel ReassignedByUser { get; set; } = null!;

        // Может не быть старого исполнителя
        public Guid? OldAssignedUserId { get; set; }
        public UserModel? OldAssignedUser { get; set; }

        // Новый исполнитель обязателен
        public Guid NewAssignedUserId { get; set; }
        public UserModel NewAssignedUser { get; set; } = null!;

        // Причина может быть не указана
        public string? Reason { get; set; }

        public DateTime ReassignedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
