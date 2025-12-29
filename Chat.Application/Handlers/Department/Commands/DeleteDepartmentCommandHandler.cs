using Chat.Application.Features;
using Chat.Common.Helpers;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class DeleteDepartmentCommandHandler
        : IRequestHandler<DeleteDepartmentCommand, Result<Unit>>
    {
        private readonly CoreDbContext _context;

        public DeleteDepartmentCommandHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Unit>> Handle(
            DeleteDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(x => x.Id == request.DepartmentId
                    && !x.IsDeleted,
                    cancellationToken);

            if (department == null)
                return Result<Unit>.Failure("Department not found");

            // Проверяем что в департаменте нет активных пользователей
            var hasUsers = await _context.Users
                .AnyAsync(x => x.DepartmentId == request.DepartmentId
                    && !x.IsDeleted,
                    cancellationToken);

            if (hasUsers)
                return Result<Unit>.Failure("Cannot delete department with active users");

            // Soft delete
            department.IsDeleted = true;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}