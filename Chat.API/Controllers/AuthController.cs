using Chat.API.Services.Facades;
using Chat.Application.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly AuthServiceFacade _authFacade;

        public AuthController(AuthServiceFacade authFacade)
        {
            _authFacade = authFacade;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var response = await _authFacade.Login(command);

            if (response == null)
                return Unauthorized(new { error = "Invalid username or password" });

            return Ok(response);
        }
    }
}