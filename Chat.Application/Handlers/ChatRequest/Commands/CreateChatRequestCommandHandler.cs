    using Chat.Application.Features;
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

            public CreateChatRequestCommandHandler(CoreDbContext context)
            {
                _context = context;
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

            return Result<Unit>.Success(Unit.Value);
            }
        }
    }