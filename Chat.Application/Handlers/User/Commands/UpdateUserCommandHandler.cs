using Chat.Application.Features;
using Chat.Common.Helpers;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<Unit>>
    {
        private readonly CoreDbContext _context;
        public UpdateUserCommandHandler(CoreDbContext context) => _context = context;

        public async Task<Result<Unit>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);
            if (user == null) return Result<Unit>.Failure("User not found");

            if (user.Username != request.Username && await _context.Users.AnyAsync(u => u.Username == request.Username && u.Id != request.UserId && !u.IsDeleted, cancellationToken))
                return Result<Unit>.Failure("Username already exists");

            if (!await _context.Departments.AnyAsync(d => d.Id == request.DepartmentId && !d.IsDeleted, cancellationToken))
                return Result<Unit>.Failure("Department not found");

            user.FullName = request.FullName;
            user.Username = request.Username;
            user.DepartmentId = request.DepartmentId;
            user.IsDepartmentAdmin = request.IsDepartmentAdmin;

            await _context.SaveChangesAsync(cancellationToken);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}