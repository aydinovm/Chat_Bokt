using Chat.Application.Features;
using Chat.Common.Helpers;
using Chat.Domain.Enums;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<Unit>>
    {
        private readonly CoreDbContext _context;
        public DeleteUserCommandHandler(CoreDbContext context) => _context = context;

        public async Task<Result<Unit>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);
            if (user == null) return Result<Unit>.Failure("User not found");

            var hasActiveChats = await _context.ChatRequests.AnyAsync(c => c.AssignedToUserId == request.UserId && c.Status != ChatRequestStatusEnum.Resolved
 && !c.IsDeleted, cancellationToken);
            if (hasActiveChats) return Result<Unit>.Failure("Cannot delete user with active assigned chats");

            user.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}