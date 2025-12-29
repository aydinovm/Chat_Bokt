using Chat.Common.Helpers;
using MediatR;

namespace Chat.Application.Features
{
    public class UpdateDepartmentCommand : BaseCommand, IRequest<Result<Unit>>
    {
        public Guid DepartmentId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
    }
}