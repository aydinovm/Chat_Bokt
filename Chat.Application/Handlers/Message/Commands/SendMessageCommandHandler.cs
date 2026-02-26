using Chat.Application.Features;
using Chat.Application.Tags;
using Chat.Common.Helpers;
using Chat.Domain.Enums;
using Chat.Domain.Persistence;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class SendMessageCommandHandler
        : IRequestHandler<SendMessageCommand, Result<Guid>>
    {
        private readonly CoreDbContext _context;
        private readonly IRealtimeNotifier _rt;

        public SendMessageCommandHandler(CoreDbContext context, IRealtimeNotifier rt)
        {
            _context = context;
            _rt = rt;
        }

        public async Task<Result<Guid>> Handle(
            SendMessageCommand request,
            CancellationToken cancellationToken)
        {
            // 1) Базовая валидация payload
            if (!Enum.IsDefined(typeof(MessageTypeEnum), request.Type))
                return Result<Guid>.Failure("Invalid message type");

            var isText = request.Type == MessageTypeEnum.Text;
            var isFile = request.Type == MessageTypeEnum.File;
            var isImage = request.Type == MessageTypeEnum.Image;



            // 2) Чат должен существовать
            var chat = await _context.ChatRequests
                .FirstOrDefaultAsync(x =>
                    x.Id == request.ChatRequestId &&
                    !x.IsDeleted,
                    cancellationToken);

            if (chat == null)
                return Result<Guid>.Failure("Chat request not found");

            // 3) Пользователь должен существовать
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == request.SenderUserId &&
                    !x.IsDeleted,
                    cancellationToken);

            if (user == null)
                return Result<Guid>.Failure("User not found");

            // 4) Запрет на сообщения в закрытый чат (если у тебя это правило есть)
            if (chat.Status == ChatRequestStatusEnum.Closed)
                return Result<Guid>.Failure("Chat is closed");

            // 5) Проверка доступа
            bool hasAccess =
                chat.FromDepartmentId == user.DepartmentId ||
                chat.ToDepartmentId == user.DepartmentId ||
                chat.AssignedToUserId == request.SenderUserId ||
                chat.CreatedByUserId == request.SenderUserId ||
                user.IsDepartmentAdmin;

            if (!hasAccess)
                return Result<Guid>.Failure("You don't have access to this chat");

            // 6) Создание сообщения
            var message = new MessageModel
            {
                Id = Guid.NewGuid(),
                ChatRequestId = request.ChatRequestId,
                SenderUserId = request.SenderUserId,
                Type = request.Type,
                Text = isText ? request.Text?.Trim() : null,
                FileUrl = (isFile || isImage) ? request.FileUrl : null,
                IsRead = false,
                SentAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _context.Messages.AddAsync(message, cancellationToken);

            // 7) Обновление статуса чата
            if (chat.Status == ChatRequestStatusEnum.Sent)
                chat.Status = ChatRequestStatusEnum.Viewed;

            chat.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            // 8) REALTIME: сообщение
            await _rt.MessageSent(chat.Id, new
            {
                id = message.Id,
                chatRequestId = message.ChatRequestId,
                senderUserId = message.SenderUserId,
                type = message.Type.ToString(),
                text = message.Text,
                fileUrl = message.FileUrl,
                isRead = message.IsRead,
                readAt = message.ReadAt,
                sentAt = message.SentAt
            }, cancellationToken);

            // 9) REALTIME: обновление чата
            await _rt.ChatUpdated(chat.Id, new
            {
                chatId = chat.Id,
                status = chat.Status.ToString(),
                modifiedDate = chat.ModifiedDate
            }, cancellationToken);

            return Result<Guid>.Success(message.Id);
        }
    }
}