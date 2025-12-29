using Chat.Common.Helpers;
using MediatR;

namespace Chat.Application.Features
{
    public class ReassignChatCommand : BaseCommand, IRequest<Result<Unit>>
    {
        public Guid ChatRequestId { get; set; }
        public Guid ReassignedByUserId { get; set; } // IT админ
        public Guid NewAssignedUserId { get; set; }
        public string Reason { get; set; }
    }
}