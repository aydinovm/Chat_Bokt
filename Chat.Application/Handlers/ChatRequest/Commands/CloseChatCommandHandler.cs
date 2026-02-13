using Chat.Application.Features;
using Chat.Application.Tags;
using Chat.Common.Helpers;
using Chat.Domain.Enums;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class CloseChatCommandHandler
        : IRequestHandler<CloseChatCommand, Result<Unit>>
    {
        private readonly CoreDbContext _context;

        public CloseChatCommandHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Unit>> Handle(CloseChatCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(x => x.Id == request.ClosedByUserId && !x.IsDeleted, cancellationToken);

            if (user == null)
                return Result<Unit>.Failure("User not found");

            if (!user.IsSuperAdmin())
                return Result<Unit>.Failure("Only super admin can close chats");

            var chat = await _context.ChatRequests
                .FirstOrDefaultAsync(x => x.Id == request.ChatRequestId && !x.IsDeleted, cancellationToken);

            if (chat == null)
                return Result<Unit>.Failure("Chat request not found");

            chat.Status = ChatRequestStatusEnum.Closed;
            chat.ClosedAt = DateTime.UtcNow;
            chat.ClosedByUserId = request.ClosedByUserId;
            chat.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
