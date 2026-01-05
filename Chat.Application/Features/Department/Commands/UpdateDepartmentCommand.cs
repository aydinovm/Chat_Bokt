using Chat.Common.Helpers;
using Chat.Domain.Enums;
using MediatR;

namespace Chat.Application.Features
{
    public class UpdateDepartmentCommand : BaseCommand, IRequest<Result<Unit>>
    {
        public Guid DepartmentId { get; set; }
        public string Name { get; set; } = null!;
        public DepartmentTypeEnum Type { get; set; }
        public bool IsActive { get; set; }
    }
}