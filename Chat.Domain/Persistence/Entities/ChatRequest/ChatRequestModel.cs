using Chat.Domain.Enums;

namespace Chat.Domain.Persistence
{
    public class ChatRequestModel : BaseEntity, IEntity
    {
        public Guid CreatedByUserId { get; set; }
        public Guid FromDepartmentId { get; set; }
        public Guid ToDepartmentId { get; set; }
        public Guid? AssignedToUserId { get; set; }

        public string Status { get; set; } // "Sent", "Viewed", "Resolved"
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public ICollection<MessageModel> Messages { get; set; } // ← Вернул!

        public bool IsDeleted { get; set; }
    }
}
