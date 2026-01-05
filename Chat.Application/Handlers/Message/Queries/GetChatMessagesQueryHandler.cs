using Chat.Application.Features;
using Chat.Application.Responces.Message;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class GetChatMessagesQueryHandler
        : IRequestHandler<GetChatMessagesQuery, List<MessageResponse>>
    {
        private readonly CoreDbContext _context;

        public GetChatMessagesQueryHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<MessageResponse>> Handle(
            GetChatMessagesQuery request,
            CancellationToken cancellationToken)
        {
            // Проверяем пользователя
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == request.UserId &&
                    !x.IsDeleted,
                    cancellationToken);

            if (user == null)
                return new List<MessageResponse>();

            // Проверяем чат
            var chat = await _context.ChatRequests
                .FirstOrDefaultAsync(x =>
                    x.Id == request.ChatRequestId &&
                    !x.IsDeleted,
                    cancellationToken);

            if (chat == null)
                return new List<MessageResponse>();

            // Проверка доступа
            bool hasAccess =
                chat.FromDepartmentId == user.DepartmentId ||
                chat.ToDepartmentId == user.DepartmentId ||
                chat.AssignedToUserId == request.UserId ||
                chat.CreatedByUserId == request.UserId ||
                user.IsDepartmentAdmin;

            if (!hasAccess)
                return new List<MessageResponse>();

            // Получаем сообщения
            return await _context.Messages
                .Where(x =>
                    x.ChatRequestId == request.ChatRequestId &&
                    !x.IsDeleted)
                .OrderBy(x => x.SentAt)
                .Select(x => new MessageResponse
                {
                    Id = x.Id,
                    ChatRequestId = x.ChatRequestId,
                    SenderUserId = x.SenderUserId,
                    Type = x.Type.ToString(),
                    Text = x.Text,
                    FileUrl = x.FileUrl,
                    IsRead = x.IsRead,
                    ReadAt = x.ReadAt,
                    SentAt = x.SentAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}
