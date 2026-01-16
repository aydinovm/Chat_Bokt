namespace Chat.Domain.Persistence
{
    public class ChatReassignmentHistoryModel : BaseEntity, IEntity
    {
        public Guid ChatRequestId { get; set; }
        public ChatRequestModel ChatRequest { get; set; } = null!;

        public Guid ReassignedByUserId { get; set; }
        public UserModel ReassignedByUser { get; set; } = null!;

        public Guid? OldAssignedUserId { get; set; }
        public UserModel? OldAssignedUser { get; set; }

        public Guid NewAssignedUserId { get; set; }
        public UserModel NewAssignedUser { get; set; } = null!;

        // ✅ NEW: междеп аудит
        public Guid? OldToDepartmentId { get; set; }
        public Guid NewToDepartmentId { get; set; }

        public string? Reason { get; set; }
        public DateTime ReassignedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
