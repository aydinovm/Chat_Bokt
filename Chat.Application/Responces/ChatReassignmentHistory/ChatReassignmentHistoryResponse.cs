namespace Chat.Application.Responces
{
    public class ChatReassignmentHistoryResponse : BaseResponce
    {
        public Guid ChatRequestId { get; set; }
        public Guid ReassignedByUserId { get; set; }
        public string ReassignedByUserName { get; set; }
        public Guid? OldAssignedUserId { get; set; }
        public string OldAssignedUserName { get; set; }
        public Guid NewAssignedUserId { get; set; }
        public string NewAssignedUserName { get; set; }
        public string Reason { get; set; }
        public DateTime ReassignedAt { get; set; }
    }
}