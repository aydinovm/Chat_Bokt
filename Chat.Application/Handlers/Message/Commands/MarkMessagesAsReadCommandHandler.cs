using Chat.Application.Features;
using Chat.Common.Helpers;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class MarkMessagesAsReadCommandHandler
        : IRequestHandler<MarkMessagesAsReadCommand, Result<Unit>>
    {
        private readonly CoreDbContext _context;

        public MarkMessagesAsReadCommandHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Unit>> Handle(
            MarkMessagesAsReadCommand request,
            CancellationToken cancellationToken)
        {
            // Проверяем доступ к чату
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == request.UserId
                    && !x.IsDeleted,
                    cancellationToken);

            if (user == null)
                return Result<Unit>.Failure("User not found");

            var chat = await _context.ChatRequests
                .FirstOrDefaultAsync(x => x.Id == request.ChatRequestId
                    && !x.IsDeleted,
                    cancellationToken);

            if (chat == null)
                return Result<Unit>.Failure("Chat not found");

            bool hasAccess = chat.FromDepartmentId == user.DepartmentId
                || chat.ToDepartmentId == user.DepartmentId
                || chat.AssignedToUserId == request.UserId;

            if (!hasAccess)
                return Result<Unit>.Failure("Access denied");

            // Помечаем непрочитанные сообщения как прочитанные
            var unreadMessages = await _context.Messages
                .Where(x => x.ChatRequestId == request.ChatRequestId
                    && x.SenderUserId != request.UserId
                    && !x.IsRead
                    && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}