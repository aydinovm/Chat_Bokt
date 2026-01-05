using Chat.Application.Responces;
using MediatR;

namespace Chat.Application.Features
{
    public class GetAllUsersQuery : IRequest<List<UserResponse>>
    {
        public Guid? DepartmentId { get; set; }
    }
}