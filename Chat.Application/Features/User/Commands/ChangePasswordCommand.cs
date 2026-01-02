using Chat.Common.Helpers;
using MediatR;

namespace Chat.Application.Features
{
    public class ChangePasswordCommand : BaseCommand, IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}