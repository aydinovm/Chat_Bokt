using Chat.Application.Features;
using Chat.Common.Helpers;
using Chat.Domain.Persistence;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class CreateUserCommandHandler
        : IRequestHandler<CreateUserCommand, Result<Guid>>
    {
        private readonly CoreDbContext _context;

        public CreateUserCommandHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(
            CreateUserCommand request,
            CancellationToken cancellationToken)
        {
            // Проверяем что username уникален
            var usernameExists = await _context.Users
                .AnyAsync(x => x.Username == request.Username
                    && !x.IsDeleted,
                    cancellationToken);

            if (usernameExists)
                return Result<Guid>.Failure("Username already exists");

            // Проверяем что департамент существует
            var departmentExists = await _context.Departments
                .AnyAsync(x => x.Id == request.DepartmentId
                    && !x.IsDeleted,
                    cancellationToken);

            if (!departmentExists)
                return Result<Guid>.Failure("Department not found");

            var user = new UserModel
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                Username = request.Username,
                Password = request.Password, // TODO: Hash password!
                DepartmentId = request.DepartmentId,
                IsDepartmentAdmin = request.IsDepartmentAdmin,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(user.Id);
        }
    }
}