using Chat.Application.Responces;
using Chat.Common.Helpers;
using MediatR;

namespace Chat.Application.Features
{
    public class LoginCommand : IRequest<Result<LoginResponse>>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}