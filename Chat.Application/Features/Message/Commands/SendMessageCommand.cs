using Chat.Application.Features;
using Chat.Common.Helpers;
using Chat.Domain.Enums;
using MediatR;

public class SendMessageCommand : BaseCommand, IRequest<Result<Guid>>
{
    public Guid ChatRequestId { get; set; }
    public Guid SenderUserId { get; set; }
    public Guid? SenderDepartmentId { get; set; }

    public MessageTypeEnum Type { get; set; }

    public string? Text { get; set; }     // ✅ nullable
    public string? FileUrl { get; set; }  // ✅ nullable
}