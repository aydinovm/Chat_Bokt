using Chat.Domain.Enums;

namespace Chat.Domain.Persistence
{

    public class MessageModel : BaseEntity, IEntity
    {
        public Guid ChatRequestId { get; set; }
        public ChatRequestModel ChatRequest { get; set; } = null!;

        public Guid SenderUserId { get; set; }
        public UserModel SenderUser { get; set; } = null!;

        public MessageTypeEnum Type { get; set; }

        public string? Text { get; set; }
        public string? FileUrl { get; set; }

        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime SentAt { get; set; }

        public bool IsDeleted { get; set; }
    }

}
