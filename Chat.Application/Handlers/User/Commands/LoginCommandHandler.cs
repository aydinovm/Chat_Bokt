using Chat.Application.Features;
using Chat.Application.Responces;
using Chat.Common.Helpers;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly CoreDbContext _context;
        public LoginCommandHandler(CoreDbContext context) => _context = context;

        public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password && !u.IsDeleted, cancellationToken);

            if (user == null) return Result<LoginResponse>.Failure("Invalid username or password");

            var response = new LoginResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                DepartmentId = user.DepartmentId,
                DepartmentName = user.Department?.Name,
                DepartmentType = user.Department?.Type.ToString(),
                IsDepartmentAdmin = user.IsDepartmentAdmin,
                Token = "", // JWT позже
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            return Result<LoginResponse>.Success(response);
        }
    }
}