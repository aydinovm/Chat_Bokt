namespace Chat.Domain.Persistence
{
    public class UserModel : BaseEntity, IEntity
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public Guid DepartmentId { get; set; }
        public bool IsDepartmentAdmin { get; set; }

        public DateTime CreatedDate { get; set; }

        public ICollection<MessageModel> SentMessages { get; set; } // ← Добавил
        public ICollection<ChatRequestModel> CreatedChats { get; set; } // ← Добавил
        public ICollection<ChatRequestModel> AssignedChats { get; set; } // ← Добавил

        public bool IsDeleted { get; set; }
    }
}
