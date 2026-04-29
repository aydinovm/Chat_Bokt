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

        public Guid? NewAssignedUserId { get; set; }   // ← было Guid, стало Guid?
        public UserModel? NewAssignedUser { get; set; } // ← было null!, стало nullable

        public Guid? OldToDepartmentId { get; set; }
        public Guid? NewToDepartmentId { get; set; }   // ← было Guid, стало Guid?

        public string? Reason { get; set; }
        public string? Comment { get; set; }           // ← добавь
        public DateTime ReassignedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}