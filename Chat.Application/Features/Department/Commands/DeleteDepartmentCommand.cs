using Chat.Common.Helpers;
using MediatR;

namespace Chat.Application.Features
{
    public class DeleteDepartmentCommand : BaseCommand, IRequest<Result<Unit>>
    {
        public Guid DepartmentId { get; set; }
    }
}