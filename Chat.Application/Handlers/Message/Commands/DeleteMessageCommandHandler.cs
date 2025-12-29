using Chat.Application.Features;
using Chat.Common.Helpers;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class DeleteMessageCommandHandler
        : IRequestHandler<DeleteMessageCommand, Result<Unit>>
    {
        private readonly CoreDbContext _context;

        public DeleteMessageCommandHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Unit>> Handle(
            DeleteMessageCommand request,
            CancellationToken cancellationToken)
        {
            var message = await _context.Messages
                .FirstOrDefaultAsync(x => x.Id == request.MessageId
                    && !x.IsDeleted,
                    cancellationToken);

            if (message == null)
                return Result<Unit>.Failure("Message not found");

            // Только отправитель может удалить своё сообщение
            if (message.SenderUserId != request.UserId)
                return Result<Unit>.Failure("You can only delete your own messages");

            // Soft delete
            message.IsDeleted = true;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}