using Chat.Application.Features;
using Chat.Application.Responces;
using MediatR;

namespace Chat.API.Services.Facades
{
    public class AuthServiceFacade
    {
        private readonly IMediator _mediator;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _config;

        public AuthServiceFacade(
            IMediator mediator,
            IJwtService jwtService,
            IConfiguration config)
        {
            _mediator = mediator;
            _jwtService = jwtService;
            _config = config;
        }

        public async Task<LoginResponse> Login(LoginCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return null;

            var user = result.Data;

            var token = _jwtService.GenerateToken(
                userId: user.UserId,
                username: user.Username,
                fullName: user.FullName,
                isDepartmentAdmin: user.IsDepartmentAdmin,
                departmentId: user.DepartmentId
            );

            var expHours = int.Parse(_config["Jwt:ExpirationHours"] ?? "8");
            user.Token = token;
            user.ExpiresAt = DateTime.UtcNow.AddHours(expHours);

            return user;
        }
    }
}