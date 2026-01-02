using Chat.Application.Features;
using Chat.Application.Responces;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class GetUserByIdQueryHandler
        : IRequestHandler<GetUserByIdQuery, UserDetailResponse>
    {
        private readonly CoreDbContext _context;

        public GetUserByIdQueryHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<UserDetailResponse> Handle(
            GetUserByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Users
                .Where(x => x.Id == request.UserId && !x.IsDeleted)
                .Select(x => new UserDetailResponse
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Username = x.Username,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = _context.Departments
                        .Where(d => d.Id == x.DepartmentId)
                        .Select(d => d.Name)
                        .FirstOrDefault(),
                    DepartmentType = _context.Departments
                        .Where(d => d.Id == x.DepartmentId)
                        .Select(d => d.Type)
                        .FirstOrDefault(),
                    IsDepartmentAdmin = x.IsDepartmentAdmin,
                    CreatedDate = x.CreatedDate,
                    CreatedChatsCount = x.CreatedChats.Count(c => !c.IsDeleted),
                    AssignedChatsCount = x.AssignedChats.Count(c => !c.IsDeleted),
                    SentMessagesCount = x.SentMessages.Count(m => !m.IsDeleted)
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}