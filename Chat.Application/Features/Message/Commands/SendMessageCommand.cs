using Chat.Common.Helpers;
using Chat.Domain.Enums;
using MediatR;

namespace Chat.Application.Features
{
    public class SendMessageCommand : BaseCommand, IRequest<Result<Guid>>
    {
        public Guid ChatRequestId { get; set; }
        public Guid SenderUserId { get; set; }
        public Guid SenderDepartmentId { get; set; }
        public MessageTypeEnum Type { get; set; } // "Text", "Image", "File"
        public string Text { get; set; }
        public string FileUrl { get; set; }
    }
}