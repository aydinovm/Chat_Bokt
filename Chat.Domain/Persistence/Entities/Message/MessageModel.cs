namespace Chat.Domain.Persistence
{
    public class MessageModel : BaseEntity, IEntity
    {
        public Guid ChatRequestId { get; set; }
        public Guid SenderDepartmentId { get; set; }

        public Guid SenderUserId { get; set; }

        public string Type { get; set; } // "Text", "Image", "File"
        public string Text { get; set; }
        public string FileUrl { get; set; }

        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime SentAt { get; set; }

        public bool IsDeleted { get; set; }
    }

}
