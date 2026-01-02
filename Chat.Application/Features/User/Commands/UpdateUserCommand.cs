using Chat.Common.Helpers;
using MediatR;

namespace Chat.Application.Features
{
    public class UpdateUserCommand : BaseCommand, IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public Guid DepartmentId { get; set; }
        public bool IsDepartmentAdmin { get; set; }
    }
}