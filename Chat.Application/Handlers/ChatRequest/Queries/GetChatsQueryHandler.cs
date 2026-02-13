using Chat.Application.Features;
using Chat.Application.Responces.ChatRequest;
using Chat.Application.Tags;
using Chat.Domain.Enums;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class GetChatsQueryHandler
        : IRequestHandler<GetChatsQuery, List<ChatRequestResponse>>
    {
        private readonly CoreDbContext _context;

        public GetChatsQueryHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatRequestResponse>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

            if (user == null)
                return new List<ChatRequestResponse>();

            bool isSuperAdmin = user.IsSuperAdmin();
            bool isDeptAdmin = user.IsDepartmentAdmin;

            IQueryable<Chat.Domain.Persistence.ChatRequestModel> query = _context.ChatRequests
                .Where(x => !x.IsDeleted);

            if (isSuperAdmin)
            {
                // ✅ SuperAdmin(Control) видит всё
            }
            else if (isDeptAdmin)
            {
                // ✅ DepartmentAdmin видит входящие своего департамента
                query = query.Where(x => x.ToDepartmentId == user.DepartmentId);
            }
            else
            {
                // ✅ Employee видит только свои (созданные им) + где он исполнитель
                query = query.Where(x => x.CreatedByUserId == user.Id || x.AssignedToUserId == user.Id);
            }

            // optional: фильтр только “открытые”
            if (request.OnlyOpen == true)
            {
                query = query.Where(x => x.Status != ChatRequestStatusEnum.Resolved
                                      && x.Status != ChatRequestStatusEnum.Closed);
            }

            return await query
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new ChatRequestResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    Status = x.Status.ToString(),
                    CreatedDate = x.CreatedDate,
                    CreatedByUserId = x.CreatedByUserId,
                    FromDepartmentId = x.FromDepartmentId,
                    ToDepartmentId = x.ToDepartmentId,
                    AssignedToUserId = x.AssignedToUserId
                })
                .ToListAsync(cancellationToken);
        }
    }
}
