namespace Chat.Application.Responces.Message
{
    public class MessageResponse : BaseResponce
    {
        public Guid ChatRequestId { get; set; }
        public Guid SenderUserId { get; set; }
        public Guid SenderDepartmentId { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public string FileUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime SentAt { get; set; }
    }
}