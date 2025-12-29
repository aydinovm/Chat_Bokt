using Chat.Application.Responces.Department;
using MediatR;

namespace Chat.Application.Features
{
    public class GetDepartmentByIdQuery : IRequest<DepartmentDetailResponse>
    {
        public Guid DepartmentId { get; set; }
    }
}