using Chat.Application.Features;
using Chat.Application.Responces;
using Chat.Common.Helpers;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class LoginCommandHandler
        : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly CoreDbContext _context;

        public LoginCommandHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<Result<LoginResponse>> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            // TODO: Hash password before comparing
            var user = await _context.Users
                .Where(x => x.Username == request.Username
                    && x.Password == request.Password
                    && !x.IsDeleted)
                .Select(x => new LoginResponse
                {
                    UserId = x.Id,
                    FullName = x.FullName,
                    Username = x.Username,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = _context.Departments
                        .Where(d => d.Id == x.DepartmentId)
                        .Select(d => d.Name)
                        .FirstOrDefault(),
                    DepartmentType = _context.Departments
                        .Where(d => d.Id == x.DepartmentId)
                        .Select(d => d.Type)
                        .FirstOrDefault(),
                    IsDepartmentAdmin = x.IsDepartmentAdmin
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                return Result<LoginResponse>.Failure("Invalid username or password");

            // TODO: Generate JWT token

            return Result<LoginResponse>.Success(user);
        }
    }
}