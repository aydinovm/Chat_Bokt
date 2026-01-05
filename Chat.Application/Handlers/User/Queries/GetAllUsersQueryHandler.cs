using Chat.Application.Features;
using Chat.Application.Responces;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserResponse>>
    {
        private readonly CoreDbContext _context;
        public GetAllUsersQueryHandler(CoreDbContext context) => _context = context;

        public async Task<List<UserResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Users.Include(u => u.Department).Where(u => !u.IsDeleted);
            if (request.DepartmentId.HasValue) query = query.Where(u => u.DepartmentId == request.DepartmentId.Value);

            return await query.Select(u => new UserResponse
            {
                Id = u.Id,
                FullName = u.FullName,
                Username = u.Username,
                DepartmentId = u.DepartmentId,
                DepartmentName = u.Department != null ? u.Department.Name : null,
                IsDepartmentAdmin = u.IsDepartmentAdmin,
                CreatedDate = u.CreatedDate
            }).OrderBy(u => u.FullName).ToListAsync(cancellationToken);
        }
    }
}