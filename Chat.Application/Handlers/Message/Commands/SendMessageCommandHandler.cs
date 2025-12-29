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
            // Проверяем существование чата
            var chat = await _context.ChatRequests
                .FirstOrDefaultAsync(x => x.Id == request.ChatRequestId
                    && !x.IsDeleted,
                    cancellationToken);

            if (chat == null)
                return Result<Guid>.Failure("Chat request not found");

            // Проверяем что отправитель имеет доступ к чату
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == request.SenderUserId
                    && !x.IsDeleted,
                    cancellationToken);

            if (user == null)
                return Result<Guid>.Failure("User not found");

            bool hasAccess = chat.FromDepartmentId == user.DepartmentId
                || chat.ToDepartmentId == user.DepartmentId
                || chat.AssignedToUserId == request.SenderUserId;

            if (!hasAccess)
                return Result<Guid>.Failure("You don't have access to this chat");

            // Создаём сообщение
            var message = new MessageModel
            {
                Id = Guid.NewGuid(),
                ChatRequestId = request.ChatRequestId,
                SenderUserId = request.SenderUserId,
                SenderDepartmentId = request.SenderDepartmentId,
                Type = request.Type,
                Text = request.Text,
                FileUrl = request.FileUrl,
                IsRead = false,
                SentAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _context.Messages.AddAsync(message, cancellationToken);

            // Обновляем статус чата на "Viewed" если был "Sent"
            if (chat.Status == "Sent")
            {
                chat.Status = "Viewed";
                chat.ModifiedDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(message.Id);
        }
    }
}