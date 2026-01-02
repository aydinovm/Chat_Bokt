using Chat.Application.Features;
using Chat.Common.Helpers;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class ChangePasswordCommandHandler
        : IRequestHandler<ChangePasswordCommand, Result<Unit>>
    {
        private readonly CoreDbContext _context;

        public ChangePasswordCommandHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Unit>> Handle(
            ChangePasswordCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == request.UserId
                    && !x.IsDeleted,
                    cancellationToken);

            if (user == null)
                return Result<Unit>.Failure("User not found");

            // TODO: Verify old password hash
            if (user.Password != request.OldPassword)
                return Result<Unit>.Failure("Old password is incorrect");

            // TODO: Hash new password
            user.Password = request.NewPassword;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}