using Chat.Application.Features;
using Chat.Common.Helpers;
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
                Description = request.Description,
                Status = "Sent", // Статус всегда "Sent" при создании
                CreatedDate = DateTime.UtcNow, // ← UtcNow
                IsDeleted = false
            };

            await _context.ChatRequests.AddAsync(chat, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}