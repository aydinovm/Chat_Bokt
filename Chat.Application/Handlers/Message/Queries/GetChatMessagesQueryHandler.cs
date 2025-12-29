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
            // Проверяем доступ пользователя к чату
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == request.UserId
                    && !x.IsDeleted,
                    cancellationToken);

            if (user == null)
                return new List<MessageResponse>();

            var chat = await _context.ChatRequests
                .FirstOrDefaultAsync(x => x.Id == request.ChatRequestId
                    && !x.IsDeleted,
                    cancellationToken);

            if (chat == null)
                return new List<MessageResponse>();

            // Проверяем что пользователь имеет доступ к чату
            bool hasAccess = chat.FromDepartmentId == user.DepartmentId
                || chat.ToDepartmentId == user.DepartmentId
                || chat.AssignedToUserId == request.UserId
                || chat.CreatedByUserId == request.UserId
                || user.IsDepartmentAdmin;

            if (!hasAccess)
                return new List<MessageResponse>();

            // Получаем сообщения
            var messages = await _context.Messages
                .Where(x => x.ChatRequestId == request.ChatRequestId
                    && !x.IsDeleted)
                .OrderBy(x => x.SentAt)
                .Select(x => new MessageResponse
                {
                    Id = x.Id,
                    ChatRequestId = x.ChatRequestId,
                    SenderUserId = x.SenderUserId,
                    SenderDepartmentId = x.SenderDepartmentId,
                    Type = x.Type,
                    Text = x.Text,
                    FileUrl = x.FileUrl,
                    IsRead = x.IsRead,
                    ReadAt = x.ReadAt,
                    SentAt = x.SentAt
                })
                .ToListAsync(cancellationToken);

            // Помечаем сообщения как прочитанные (в фоне)
            _ = Task.Run(async () =>
            {
                await MarkMessagesAsRead(request.ChatRequestId, request.UserId, cancellationToken);
            }, cancellationToken);

            return messages;
        }

        private async Task MarkMessagesAsRead(
            Guid chatRequestId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            try
            {
                var unreadMessages = await _context.Messages
                    .Where(x => x.ChatRequestId == chatRequestId
                        && x.SenderUserId != userId
                        && !x.IsRead
                        && !x.IsDeleted)
                    .ToListAsync(cancellationToken);

                if (unreadMessages.Any())
                {
                    foreach (var message in unreadMessages)
                    {
                        message.IsRead = true;
                        message.ReadAt = DateTime.UtcNow;
                    }

                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
            catch
            {
                // Игнорируем ошибки при пометке прочитанных
            }
        }
    }
}