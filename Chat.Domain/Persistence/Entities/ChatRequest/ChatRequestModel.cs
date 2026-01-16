using Chat.Domain.Enums;

namespace Chat.Domain.Persistence
{
    public class ChatRequestModel : BaseEntity, IEntity
    {
        public Guid CreatedByUserId { get; set; }
        public UserModel CreatedByUser { get; set; } = null!;

        public Guid FromDepartmentId { get; set; }
        public Guid ToDepartmentId { get; set; }

        public Guid? AssignedToUserId { get; set; }
        public UserModel? AssignedToUser { get; set; }

        public ChatRequestStatusEnum Status { get; set; }

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // ✅ NEW: аудит
        public DateTime? ResolvedAt { get; set; }
        public Guid? ResolvedByUserId { get; set; }

        public DateTime? ClosedAt { get; set; }
        public Guid? ClosedByUserId { get; set; }

        public ICollection<MessageModel> Messages { get; set; } = new HashSet<MessageModel>();
        public ICollection<ChatReassignmentHistoryModel> ReassignmentHistory { get; set; } = new HashSet<ChatReassignmentHistoryModel>();

        public bool IsDeleted { get; set; }
    }
}
