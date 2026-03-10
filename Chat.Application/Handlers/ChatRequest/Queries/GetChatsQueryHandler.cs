using Chat.Application.Features;
using Chat.Application.Responces.ChatRequest;
using Chat.Application.Tags;
using Chat.Domain.Enums;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class GetChatsQueryHandler : IRequestHandler<GetChatsQuery, List<ChatRequestResponse>>
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
                // SuperAdmin видит всё
            }
            else if (isDeptAdmin)
            {
                query = query.Where(x => x.ToDepartmentId == user.DepartmentId || x.FromDepartmentId == user.DepartmentId);
            }
            else
            {
                query = query.Where(x => x.CreatedByUserId == user.Id || x.AssignedToUserId == user.Id);
            }

            if (request.OnlyOpen == true)
            {
                query = query.Where(x => x.Status != ChatRequestStatusEnum.Resolved
                                      && x.Status != ChatRequestStatusEnum.Closed);
            }

            // ✅ сортировка по последнему сообщению, если сообщений нет — по CreatedDate
            query = query.OrderByDescending(x =>
                _context.Messages
                    .Where(m => m.ChatRequestId == x.Id)                 // <-- проверь имя поля
                    .Max(m => (DateTime?)m.SentAt)                       // <-- проверь имя поля
                ?? x.CreatedDate
            );

            return await query
                .Select(x => new ChatRequestResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    Status = x.Status.ToString(),
                    CreatedDate = x.CreatedDate,
                    CreatedByUserId = x.CreatedByUserId,
                    FromDepartmentId = x.FromDepartmentId,
                    ToDepartmentId = x.ToDepartmentId,

                    AssignedToUserId = x.AssignedToUserId,

                    AssignedToFullName = _context.Users
                        .Where(u => u.Id == x.AssignedToUserId && !u.IsDeleted)
                        .Select(u => u.FullName)
                        .FirstOrDefault(),

                    AssignedToUserName = _context.Users
                        .Where(u => u.Id == x.AssignedToUserId && !u.IsDeleted)
                        .Select(u => u.Username)
                        .FirstOrDefault(),

                    // ✅ last message fields
                    LastMessageText = _context.Messages
                        .Where(m => m.ChatRequestId == x.Id)             // <-- проверь имя поля
                        .OrderByDescending(m => m.SentAt)                 // <-- проверь имя поля
                        .Select(m => m.Text ?? (m.FileUrl != null ? "Файл" : "")) // <-- проверь имена
                        .FirstOrDefault(),

                    LastMessageSenderUserId = _context.Messages
                        .Where(m => m.ChatRequestId == x.Id)
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => (Guid?)m.SenderUserId)              // <-- проверь имя поля
                        .FirstOrDefault(),

                    LastMessageIsRead = _context.Messages
                        .Where(m => m.ChatRequestId == x.Id)
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => m.IsRead)                           // <-- проверь имя поля
                        .FirstOrDefault(),

                    LastMessageAt = _context.Messages
                        .Where(m => m.ChatRequestId == x.Id)
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => (DateTime?)m.SentAt)
                        .FirstOrDefault(),
                })
                .ToListAsync(cancellationToken);
        }
    }
}