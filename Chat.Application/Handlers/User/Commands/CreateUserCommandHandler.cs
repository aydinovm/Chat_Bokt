using Chat.Application.Features;
using Chat.Common.Helpers;
using Chat.Domain.Persistence;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
    {
        private readonly CoreDbContext _context;
        public CreateUserCommandHandler(CoreDbContext context) => _context = context;

        public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username && !u.IsDeleted, cancellationToken))
                return Result<Guid>.Failure("Username already exists");

            if (!await _context.Departments.AnyAsync(d => d.Id == request.DepartmentId && !d.IsDeleted, cancellationToken))
                return Result<Guid>.Failure("Department not found");

            var user = new UserModel
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                Username = request.Username,
                Password = request.Password,
                DepartmentId = request.DepartmentId,
                IsDepartmentAdmin = request.IsDepartmentAdmin,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false,
                SentMessages = new HashSet<MessageModel>(),
                CreatedChats = new HashSet<ChatRequestModel>(),
                AssignedChats = new HashSet<ChatRequestModel>()
            };

            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(user.Id);
        }
    }
}