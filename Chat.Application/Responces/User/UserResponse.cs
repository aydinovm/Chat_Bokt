namespace Chat.Application.Responces
{
    public class UserResponse : BaseResponce
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public bool IsDepartmentAdmin { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class UserDetailResponse : BaseResponce
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public Guid DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? DepartmentType { get; set; }
        public bool IsDepartmentAdmin { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedChatsCount { get; set; }
        public int AssignedChatsCount { get; set; }
        public int SentMessagesCount { get; set; }
    }
}
