namespace Chat.Application.Responces
{
    public class UserResponse : BaseResponce
    {
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
