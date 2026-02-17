using Chat.Application.Features;
using Chat.Application.Tags;
using Chat.Common.Helpers;
using Chat.Domain.Enums;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class MarkMessagesAsReadCommandHandler
        : IRequestHandler<MarkMessagesAsReadCommand, Result<Unit>>
    {
        private readonly CoreDbContext _context;
        private readonly IRealtimeNotifier _rt;

        public MarkMessagesAsReadCommandHandler(CoreDbContext context, IRealtimeNotifier rt)
        {
            _context = context;
            _rt = rt;
        }

        public async Task<Result<Unit>> Handle(
            MarkMessagesAsReadCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == request.UserId && !x.IsDeleted, cancellationToken);

            if (user == null)
                return Result<Unit>.Failure("User not found");

            var chat = await _context.ChatRequests
                .FirstOrDefaultAsync(x => x.Id == request.ChatRequestId && !x.IsDeleted, cancellationToken);

            if (chat == null)
                return Result<Unit>.Failure("Chat not found");

            bool hasAccess =
                chat.FromDepartmentId == user.DepartmentId
                || chat.ToDepartmentId == user.DepartmentId
                || chat.AssignedToUserId == request.UserId;

            if (!hasAccess)
                return Result<Unit>.Failure("Access denied");

            var unreadMessages = await _context.Messages
                .Where(x => x.ChatRequestId == request.ChatRequestId
                    && x.SenderUserId != request.UserId
                    && !x.IsRead
                    && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            var ids = new List<Guid>();

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
                ids.Add(message.Id);
            }

            if (chat.Status == ChatRequestStatusEnum.Sent)
            {
                chat.Status = ChatRequestStatusEnum.Viewed;
                chat.ModifiedDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);

            // ✅ REALTIME: отправителю/всем в чате — какие сообщения прочитаны
            if (ids.Count > 0)
            {
                await _rt.MessagesRead(chat.Id, new
                {
                    chatId = chat.Id,
                    readByUserId = request.UserId,
                    messageIds = ids,
                    readAt = DateTime.UtcNow
                }, cancellationToken);

                await _rt.ChatUpdated(chat.Id, new
                {
                    chatId = chat.Id,
                    status = chat.Status.ToString(),
                    modifiedDate = chat.ModifiedDate
                }, cancellationToken);
            }

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
