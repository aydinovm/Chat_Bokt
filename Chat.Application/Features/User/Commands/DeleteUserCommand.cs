using Chat.Common.Helpers;
using MediatR;

namespace Chat.Application.Features
{
    public class DeleteUserCommand : BaseCommand, IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
    }
}