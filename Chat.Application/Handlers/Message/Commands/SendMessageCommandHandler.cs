using Chat.Application.Features;
using Chat.Common.Helpers;
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

        public SendMessageCommandHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(
            SendMessageCommand request,
            CancellationToken cancellationToken)
        {
            // Проверяем чат
            var chat = await _context.ChatRequests
                .FirstOrDefaultAsync(x =>
                    x.Id == request.ChatRequestId &&
                    !x.IsDeleted,
                    cancellationToken);

            if (chat == null)
                return Result<Guid>.Failure("Chat request not found");

            // Проверяем пользователя
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == request.SenderUserId &&
                    !x.IsDeleted,
                    cancellationToken);

            if (user == null)
                return Result<Guid>.Failure("User not found");

            // Проверка доступа
            bool hasAccess =
                chat.FromDepartmentId == user.DepartmentId ||
                chat.ToDepartmentId == user.DepartmentId ||
                chat.AssignedToUserId == request.SenderUserId ||
                chat.CreatedByUserId == request.SenderUserId ||
                user.IsDepartmentAdmin;

            if (!hasAccess)
                return Result<Guid>.Failure("You don't have access to this chat");

            // Создаём сообщение
            var message = new MessageModel
            {
                Id = Guid.NewGuid(),
                ChatRequestId = request.ChatRequestId,
                SenderUserId = request.SenderUserId,
                Type = request.Type,
                Text = request.Text,
                FileUrl = request.FileUrl,
                IsRead = false,
                SentAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _context.Messages.AddAsync(message, cancellationToken);

            // Если чат был "Sent" → становится "Viewed"
            if (chat.Status == Chat.Domain.Enums.ChatRequestStatusEnum.Sent)
            {
                chat.Status = Chat.Domain.Enums.ChatRequestStatusEnum.Viewed;
                chat.ModifiedDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(message.Id);
        }
    }
}
