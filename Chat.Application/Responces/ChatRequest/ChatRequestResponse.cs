using Chat.Application.Responces;

public class ChatRequestResponse : BaseResponce
{
    public Guid CreatedByUserId { get; set; }
    public Guid FromDepartmentId { get; set; }
    public Guid ToDepartmentId { get; set; }

    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToFullName { get; set; }
    public string? AssignedToUserName { get; set; }

    public string Status { get; set; }
    public string Title { get; set; }
    public DateTime CreatedDate { get; set; }

    // ✅ ДОБАВИТЬ
    public string? LastMessageText { get; set; }
    public Guid? LastMessageSenderUserId { get; set; }
    public bool LastMessageIsRead { get; set; }
    public DateTime? LastMessageAt { get; set; }
}