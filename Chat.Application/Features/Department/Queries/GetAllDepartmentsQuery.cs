using Chat.Application.Responces.Department;
using MediatR;

namespace Chat.Application.Features
{
    public class GetAllDepartmentsQuery : IRequest<List<DepartmentResponse>>
    {
        public bool? IsActive { get; set; } // Фильтр по активности (опционально)
    }
}