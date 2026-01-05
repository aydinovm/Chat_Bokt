using Chat.Application.Features;
using Chat.Common.Helpers;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class UpdateDepartmentCommandHandler
        : IRequestHandler<UpdateDepartmentCommand, Result<Unit>>
    {
        private readonly CoreDbContext _context;

        public UpdateDepartmentCommandHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Unit>> Handle(
            UpdateDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(x =>
                    x.Id == request.DepartmentId &&
                    !x.IsDeleted,
                    cancellationToken);

            if (department == null)
                return Result<Unit>.Failure("Department not found");

            var nameExists = await _context.Departments
                .AnyAsync(x =>
                    x.Name == request.Name &&
                    x.Id != request.DepartmentId &&
                    !x.IsDeleted,
                    cancellationToken);

            if (nameExists)
                return Result<Unit>.Failure("Department with this name already exists");

            department.Name = request.Name;
            department.Type = request.Type; // ✅ enum
            department.IsActive = request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }

    }
}