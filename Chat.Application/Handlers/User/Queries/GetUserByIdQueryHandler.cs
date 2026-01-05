using Chat.Application.Features;
using Chat.Application.Responces;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDetailResponse>
    {
        private readonly CoreDbContext _context;
        public GetUserByIdQueryHandler(CoreDbContext context) => _context = context;

        public async Task<UserDetailResponse> Handle(
            GetUserByIdQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.CreatedChats)
                .Include(u => u.AssignedChats)
                .Include(u => u.SentMessages)
                .FirstOrDefaultAsync(
                    u => u.Id == request.UserId && !u.IsDeleted,
                    cancellationToken);

            if (user == null)
                return null;

            return new UserDetailResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                DepartmentId = user.DepartmentId,
                DepartmentName = user.Department?.Name,
                DepartmentType = user.Department?.Type.ToString(),
                IsDepartmentAdmin = user.IsDepartmentAdmin,
                CreatedDate = user.CreatedDate,
                CreatedChatsCount = user.CreatedChats.Count(c => !c.IsDeleted),
                AssignedChatsCount = user.AssignedChats.Count(c => !c.IsDeleted),
                SentMessagesCount = user.SentMessages.Count(m => !m.IsDeleted)
            };
        }

    }
}