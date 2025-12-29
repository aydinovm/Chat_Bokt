using Chat.Common.Helpers;
using MediatR;

namespace Chat.Application.Features
{
    public class MarkMessagesAsReadCommand : BaseCommand, IRequest<Result<Unit>>
    {
        public Guid ChatRequestId { get; set; }
        public Guid UserId { get; set; }
    }
}