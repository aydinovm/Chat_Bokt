using Chat.Application.Responces.Department;
using MediatR;

namespace Chat.Application.Features
{
    public class GetAllDepartmentsQuery : IRequest<List<DepartmentDetailResponse>>
    {
        public bool? IsActive { get; set; } 
    }
}