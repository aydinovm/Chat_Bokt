using Chat.Application.Features;
using Chat.Application.Tags;
using Chat.Common.Helpers;
using Chat.Domain.Enums;
using Chat.Domain.Persistence;
using Chat.Persistence;
using MediatR;

namespace Chat.Application.Handlers
{
    public class CreateChatRequestCommandHandler
        : IRequestHandler<CreateChatRequestCommand, Result<Unit>>
    {
        private readonly CoreDbContext _context;
        private readonly IRealtimeNotifier _rt;

        public CreateChatRequestCommandHandler(CoreDbContext context, IRealtimeNotifier rt)
        {
            _context = context;
            _rt = rt;
        }

        public async Task<Result<Unit>> Handle(
            CreateChatRequestCommand request,
            CancellationToken cancellationToken)
        {
            var chat = new ChatRequestModel
            {
                Id = Guid.NewGuid(),
                CreatedByUserId = request.CreatedByUserId,
                FromDepartmentId = request.FromDepartmentId,
                ToDepartmentId = request.ToDepartmentId,
                Title = request.Title,
                Status = ChatRequestStatusEnum.Sent,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            var firstMessage = new MessageModel
            {
                Id = Guid.NewGuid(),
                ChatRequestId = chat.Id,
                SenderUserId = request.CreatedByUserId,
                Type = MessageTypeEnum.Text,
                Text = request.Description,
                IsRead = false,
                SentAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _context.ChatRequests.AddAsync(chat, cancellationToken);
            await _context.Messages.AddAsync(firstMessage, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // ✅ REALTIME: департамент и создатель
            await _rt.ChatCreated(chat.Id, new
            {
                id = chat.Id,
                title = chat.Title,
                status = chat.Status.ToString(),
                createdByUserId = chat.CreatedByUserId,
                fromDepartmentId = chat.FromDepartmentId,
                toDepartmentId = chat.ToDepartmentId,
                createdDate = chat.CreatedDate
            }, cancellationToken);

            // Новому департаменту пнуть обновление списка
            await _rt.NotifyDepartment(chat.ToDepartmentId, "InboxChatCreated", new
            {
                chatId = chat.Id
            }, cancellationToken);

            // Создателю тоже можно уведомление
            await _rt.NotifyUser(chat.CreatedByUserId, "MyChatCreated", new
            {
                chatId = chat.Id
            }, cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
