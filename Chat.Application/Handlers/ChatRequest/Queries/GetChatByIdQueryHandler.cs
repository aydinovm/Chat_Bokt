using Chat.Application.Features;
using Chat.Application.Responces.ChatRequest;
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

        public async Task<ChatRequestDetailResponse> Handle(
            GetChatByIdQuery request,
            CancellationToken cancellationToken)
        {
            // Проверяем доступ пользователя
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == request.UserId
                    && !x.IsDeleted,
                    cancellationToken);

            if (user == null)
                return null;

            var chat = await _context.ChatRequests
                .Where(x => x.Id == request.ChatRequestId && !x.IsDeleted)
                .Select(x => new
                {
                    Chat = x,
                    TotalMessages = x.Messages.Count(m => !m.IsDeleted),
                    UnreadMessages = x.Messages.Count(m => !m.IsDeleted
                        && !m.IsRead
                        && m.SenderUserId != request.UserId)
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (chat == null)
                return null;

            // Проверяем что пользователь имеет доступ к чату
            bool hasAccess = chat.Chat.FromDepartmentId == user.DepartmentId
                || chat.Chat.ToDepartmentId == user.DepartmentId
                || chat.Chat.AssignedToUserId == request.UserId
                || chat.Chat.CreatedByUserId == request.UserId
                || user.IsDepartmentAdmin; // IT админ видит все

            if (!hasAccess)
                return null;

            return new ChatRequestDetailResponse
            {
                Id = chat.Chat.Id,
                CreatedByUserId = chat.Chat.CreatedByUserId,
                FromDepartmentId = chat.Chat.FromDepartmentId,
                ToDepartmentId = chat.Chat.ToDepartmentId,
                AssignedToUserId = chat.Chat.AssignedToUserId,
                Status = chat.Chat.Status,
                Title = chat.Chat.Title,
                Description = chat.Chat.Description,
                CreatedDate = chat.Chat.CreatedDate,
                ModifiedDate = chat.Chat.ModifiedDate,
                TotalMessagesCount = chat.TotalMessages,
                UnreadMessagesCount = chat.UnreadMessages
            };
        }
    }
}