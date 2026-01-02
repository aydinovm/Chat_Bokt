using Chat.Application.Features;
using Chat.Application.Responces;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class GetAllUsersQueryHandler
        : IRequestHandler<GetAllUsersQuery, List<UserResponse>>
    {
        private readonly CoreDbContext _context;

        public GetAllUsersQueryHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserResponse>> Handle(
            GetAllUsersQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.Users
                .Where(x => !x.IsDeleted);

            // Фильтр по департаменту (если указан)
            if (request.DepartmentId.HasValue)
            {
                query = query.Where(x => x.DepartmentId == request.DepartmentId.Value);
            }

            return await query
                .Select(x => new UserResponse
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Username = x.Username,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = _context.Departments
                        .Where(d => d.Id == x.DepartmentId)
                        .Select(d => d.Name)
                        .FirstOrDefault(),
                    IsDepartmentAdmin = x.IsDepartmentAdmin,
                    CreatedDate = x.CreatedDate
                })
                .OrderBy(x => x.FullName)
                .ToListAsync(cancellationToken);
        }
    }
}