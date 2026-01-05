using Chat.Application.Features;
using Chat.Application.Responces.Department;
using Chat.Domain.Enums;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class GetAllDepartmentsQueryHandler
        : IRequestHandler<GetAllDepartmentsQuery, List<DepartmentDetailResponse>>
    {
        private readonly CoreDbContext _context;

        public GetAllDepartmentsQueryHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentDetailResponse>> Handle(
            GetAllDepartmentsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.Departments
                .Where(x => !x.IsDeleted);

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            return await query
                .Select(x => new DepartmentDetailResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = x.Type.ToString(), // 👈 enum → string ТОЛЬКО В RESPONSE
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
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
