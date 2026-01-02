using Chat.Application.Features;
using Chat.Common.Helpers;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class UpdateUserCommandHandler
        : IRequestHandler<UpdateUserCommand, Result<Unit>>
    {
        private readonly CoreDbContext _context;

        public UpdateUserCommandHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Unit>> Handle(
            UpdateUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == request.UserId
                    && !x.IsDeleted,
                    cancellationToken);

            if (user == null)
                return Result<Unit>.Failure("User not found");

            // Проверяем что username уникален (если изменился)
            if (user.Username != request.Username)
            {
                var usernameExists = await _context.Users
                    .AnyAsync(x => x.Username == request.Username
                        && x.Id != request.UserId
                        && !x.IsDeleted,
                        cancellationToken);

                if (usernameExists)
                    return Result<Unit>.Failure("Username already exists");
            }

            // Проверяем что департамент существует
            var departmentExists = await _context.Departments
                .AnyAsync(x => x.Id == request.DepartmentId
                    && !x.IsDeleted,
                    cancellationToken);

            if (!departmentExists)
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