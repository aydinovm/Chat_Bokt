namespace Chat.Application.Responces.ChatRequest
{
    public class ChatRequestDetailResponse : BaseResponce
    {
        public Guid CreatedByUserId { get; set; }
        public Guid FromDepartmentId { get; set; }
        public Guid ToDepartmentId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Дополнительная информация
        public int UnreadMessagesCount { get; set; }
        public int TotalMessagesCount { get; set; }
    }
}