using Chat.Domain.Enums;

namespace Chat.Domain.Persistence
{
    public class ChatRequestModel : BaseEntity, IEntity
    {
        // Кто создал — ВСЕГДА есть
        public Guid CreatedByUserId { get; set; }
        public UserModel CreatedByUser { get; set; } = null!;

        // Откуда / куда — ВСЕГДА есть
        public Guid FromDepartmentId { get; set; }
        public Guid ToDepartmentId { get; set; }

        // Исполнитель может быть не назначен
        public Guid? AssignedToUserId { get; set; }
        public UserModel? AssignedToUser { get; set; }

        // Статус обязателен
        public ChatRequestStatusEnum Status { get; set; }

        // Контент обязателен
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        // Даты
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Навигации
        public ICollection<MessageModel> Messages { get; set; }
            = new HashSet<MessageModel>();

        public ICollection<ChatReassignmentHistoryModel> ReassignmentHistory { get; set; }
            = new HashSet<ChatReassignmentHistoryModel>();

        public bool IsDeleted { get; set; }
    }
}
