namespace Chat.Domain.Persistence
{
    public class UserModel : BaseEntity, IEntity
    {
        // Обязательные данные
        public string FullName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;

        // Связь с департаментом — ВСЕГДА есть
        public Guid DepartmentId { get; set; }
        public DepartmentModel Department { get; set; } = null!;

        public bool IsDepartmentAdmin { get; set; }

        // Заполняется при создании
        public DateTime CreatedDate { get; set; }

        // Навигации (никогда не null)
        public ICollection<ChatRequestModel> CreatedChats { get; set; }
            = new HashSet<ChatRequestModel>();

        public ICollection<ChatRequestModel> AssignedChats { get; set; }
            = new HashSet<ChatRequestModel>();

        public ICollection<MessageModel> SentMessages { get; set; }
            = new HashSet<MessageModel>();

        // Soft delete
        public bool IsDeleted { get; set; }
    }
}
