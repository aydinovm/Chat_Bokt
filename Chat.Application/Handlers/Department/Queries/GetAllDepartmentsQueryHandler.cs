using Chat.Application.Features;
using Chat.Application.Responces.Department;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class GetAllDepartmentsQueryHandler
        : IRequestHandler<GetAllDepartmentsQuery, List<DepartmentResponse>>
    {
        private readonly CoreDbContext _context;

        public GetAllDepartmentsQueryHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentResponse>> Handle(
            GetAllDepartmentsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.Departments
                .Where(x => !x.IsDeleted);

            // Фильтр по активности (если указан)
            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            return await query
                .Select(x => new DepartmentResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = x.Type,
                    IsActive = x.IsActive,
                    UsersCount = x.Users.Count(u => !u.IsDeleted)
                })
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }
    }
}