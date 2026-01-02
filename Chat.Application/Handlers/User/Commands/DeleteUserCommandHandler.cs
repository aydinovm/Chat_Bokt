using Chat.Application.Features;
using Chat.Common.Helpers;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class DeleteUserCommandHandler
        : IRequestHandler<DeleteUserCommand, Result<Unit>>
    {
        private readonly CoreDbContext _context;

        public DeleteUserCommandHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Unit>> Handle(
            DeleteUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == request.UserId
                    && !x.IsDeleted,
                    cancellationToken);

            if (user == null)
                return Result<Unit>.Failure("User not found");

            // Проверяем что у пользователя нет активных чатов
            var hasActiveChats = await _context.ChatRequests
                .AnyAsync(x => x.AssignedToUserId == request.UserId
                    && x.Status != "Resolved"
                    && !x.IsDeleted,
                    cancellationToken);

            if (hasActiveChats)
                return Result<Unit>.Failure("Cannot delete user with active assigned chats");

            // Soft delete
            user.IsDeleted = true;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}