using Chat.Application.Features;
using Chat.Common.Helpers;
using MediatR;

public class ReassignChatCommand : BaseCommand, IRequest<Result<Unit>>
{
    public Guid ChatRequestId { get; set; }
    public Guid ReassignedByUserId { get; set; }
    public Guid? NewAssignedUserId { get; set; }    // nullable
    public Guid? NewToDepartmentId { get; set; }    // добавь
    public string Reason { get; set; }
    public string? Comment { get; set; }            // добавь если нет
}