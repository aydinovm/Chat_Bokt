using Chat.Common.Helpers;
using MediatR;

namespace Chat.Application.Features
{
    public class CreateDepartmentCommand : BaseCommand, IRequest<Result<Guid>>
    {
        public string Name { get; set; }
        public string Type { get; set; } // "HR", "Finance", "Marketing", "IT"
    }
}