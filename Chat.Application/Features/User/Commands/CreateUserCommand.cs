using Chat.Common.Helpers;
using MediatR;

namespace Chat.Application.Features
{
    public class CreateUserCommand : BaseCommand, IRequest<Result<Guid>>
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Guid DepartmentId { get; set; }
        public bool IsDepartmentAdmin { get; set; }
    }
}