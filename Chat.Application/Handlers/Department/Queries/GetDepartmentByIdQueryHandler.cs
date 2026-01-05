using Chat.Application.Features;
using Chat.Application.Responces.Department;
using Chat.Domain.Enums;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class GetDepartmentByIdQueryHandler
        : IRequestHandler<GetDepartmentByIdQuery, DepartmentDetailResponse>
    {
        private readonly CoreDbContext _context;

        public GetDepartmentByIdQueryHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<DepartmentDetailResponse> Handle(
            GetDepartmentByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Departments
                .Where(x => x.Id == request.DepartmentId && !x.IsDeleted)
                .Select(x => new DepartmentDetailResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = x.Type.ToString(),
                    IsActive = x.IsActive,
                    UsersCount = x.Users.Count(u => !u.IsDeleted),
                    ActiveChatsCount = _context.ChatRequests.Count(c =>
                        (c.FromDepartmentId == x.Id || c.ToDepartmentId == x.Id)
                        && c.Status != ChatRequestStatusEnum.Resolved
                        && !c.IsDeleted),
                    Users = x.Users
                        .Where(u => !u.IsDeleted)
                        .Select(u => new DepartmentUserResponse
                        {
                            Id = u.Id,
                            FullName = u.FullName,
                            Username = u.Username,
                            IsDepartmentAdmin = u.IsDepartmentAdmin
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}