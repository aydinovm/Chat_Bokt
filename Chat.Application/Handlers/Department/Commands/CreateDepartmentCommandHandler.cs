using Chat.Application.Features;
using Chat.Common.Helpers;
using Chat.Domain.Persistence;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class CreateDepartmentCommandHandler
        : IRequestHandler<CreateDepartmentCommand, Result<Guid>>
    {
        private readonly CoreDbContext _context;

        public CreateDepartmentCommandHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(
            CreateDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            var exists = await _context.Departments
                .AnyAsync(x =>
                    x.Name == request.Name &&
                    !x.IsDeleted,
                    cancellationToken);

            if (exists)
                return Result<Guid>.Failure("Department with this name already exists");

            var department = new DepartmentModel
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Type = request.Type, // ✅ enum → enum
                IsActive = true,
                IsDeleted = false
            };

            await _context.Departments.AddAsync(department, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(department.Id);
        }

    }
}