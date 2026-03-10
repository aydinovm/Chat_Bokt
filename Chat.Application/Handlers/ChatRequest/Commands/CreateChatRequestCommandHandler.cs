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
                // ✅ Берём departmentId из самого пользователя
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == request.CreatedByUserId && !u.IsDeleted, cancellationToken);

                if (user == null)
                    return Result<Unit>.Failure("Пользователь не найден");

                var chat = new ChatRequestModel
                {
                    Id = Guid.NewGuid(),
                    CreatedByUserId = request.CreatedByUserId,
                    FromDepartmentId = user.DepartmentId, // ✅ из пользователя, не с фронта
                    ToDepartmentId = request.ToDepartmentId,
                    Title = request.Title,
                    Status = ChatRequestStatusEnum.Sent,
                    CreatedDate = DateTime.UtcNow,
                    Description = request.Description ?? string.Empty,
                    IsDeleted = false
                };

                var firstMessage = new MessageModel
                {
                    Id = Guid.NewGuid(),
                    ChatRequestId = chat.Id,
                    SenderUserId = request.CreatedByUserId,
                    Type = MessageTypeEnum.Text,
                    Text = request.Description ?? String.Empty,
                    IsRead = false,
                    SentAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _context.ChatRequests.AddAsync(chat, cancellationToken);
                await _context.Messages.AddAsync(firstMessage, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

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

                await _rt.NotifyDepartment(chat.ToDepartmentId, "InboxChatCreated", new
                {
                    chatId = chat.Id
                }, cancellationToken);

                await _rt.NotifyUser(chat.CreatedByUserId, "MyChatCreated", new
                {
                    chatId = chat.Id
                }, cancellationToken);

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }