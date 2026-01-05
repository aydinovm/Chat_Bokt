using Chat.Common.Helpers;
using Chat.Domain.Enums;
using MediatR;

namespace Chat.Application.Features
{
    public class CreateDepartmentCommand : BaseCommand, IRequest<Result<Guid>>
    {
        public string Name { get; set; } = null!;
        public DepartmentTypeEnum Type { get; set; }
    }
}