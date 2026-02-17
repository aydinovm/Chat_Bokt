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
            var chat = await _context.ChatRequests
                .FirstOrDefaultAsync(x =>
                    x.Id == request.ChatRequestId &&
                    !x.IsDeleted,
                    cancellationToken);

            if (chat == null)
                return Result<Guid>.Failure("Chat request not found");

            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == request.SenderUserId &&
                    !x.IsDeleted,
                    cancellationToken);

            if (user == null)
                return Result<Guid>.Failure("User not found");

            bool hasAccess =
                chat.FromDepartmentId == user.DepartmentId ||
                chat.ToDepartmentId == user.DepartmentId ||
                chat.AssignedToUserId == request.SenderUserId ||
                chat.CreatedByUserId == request.SenderUserId ||
                user.IsDepartmentAdmin;

            if (!hasAccess)
                return Result<Guid>.Failure("You don't have access to this chat");

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

            if (chat.Status == ChatRequestStatusEnum.Sent)
                chat.Status = ChatRequestStatusEnum.Viewed;

            chat.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            // ✅ REALTIME
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

            // ✅ для списка чатов (если фронт показывает last message / counters)
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
