using Chat.Application.Features;
using Chat.Application.Responces.ChatRequest;
using Chat.Application.Tags;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class GetChatByIdQueryHandler
        : IRequestHandler<GetChatByIdQuery, ChatRequestDetailResponse>
    {
        private readonly CoreDbContext _context;

        public GetChatByIdQueryHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<ChatRequestDetailResponse> Handle(GetChatByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(x => x.Id == request.UserId && !x.IsDeleted, cancellationToken);

            if (user == null)
                return null;

            bool isSuperAdmin = user.IsSuperAdmin();

            var chatWrap = await _context.ChatRequests
                .Where(x => x.Id == request.ChatRequestId && !x.IsDeleted)
                .Select(x => new
                {
                    Chat = x,
                    TotalMessages = x.Messages.Count(m => !m.IsDeleted),
                    UnreadMessages = x.Messages.Count(m =>
                        !m.IsDeleted &&
                        !m.IsRead &&
                        m.SenderUserId != request.UserId)
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (chatWrap == null)
                return null;

            var chat = chatWrap.Chat;

            bool isDeptAdmin = user.IsDepartmentAdmin;

            bool hasAccess =
                isSuperAdmin
                || (isDeptAdmin && chat.ToDepartmentId == user.DepartmentId) // ✅ департ-админ только свои входящие
                || chat.AssignedToUserId == request.UserId
                || chat.CreatedByUserId == request.UserId
                || chat.FromDepartmentId == user.DepartmentId
                || chat.ToDepartmentId == user.DepartmentId;

            if (!hasAccess)
                return null;

            return new ChatRequestDetailResponse
            {
                Id = chat.Id,
                CreatedByUserId = chat.CreatedByUserId,
                FromDepartmentId = chat.FromDepartmentId,
                ToDepartmentId = chat.ToDepartmentId,
                AssignedToUserId = chat.AssignedToUserId,
                Status = chat.Status.ToString(),
                Title = chat.Title,
                Description = chat.Description,
                CreatedDate = chat.CreatedDate,
                ModifiedDate = chat.ModifiedDate,
                TotalMessagesCount = chatWrap.TotalMessages,
                UnreadMessagesCount = chatWrap.UnreadMessages
            };
        }
    }
}
