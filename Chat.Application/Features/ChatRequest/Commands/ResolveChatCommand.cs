using Chat.Common.Helpers;
using MediatR;

namespace Chat.Application.Features
{
    public class ResolveChatCommand : BaseCommand, IRequest<Result<Unit>>
    {
        public Guid ChatRequestId { get; set; }
        public Guid ResolvedByUserId { get; set; }
    }
}