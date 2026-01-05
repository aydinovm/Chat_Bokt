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

        // =========================
        // POST: api/auth/login
        // =========================
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var response = await _authFacade.Login(command);

            if (response == null)
                return Unauthorized(new { error = "Invalid username or password" });

            return Ok(response);
        }

        // =========================
        // POST: api/auth/change-password
        // =========================
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            // Берём текущего пользователя из JWT
            command.UserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok();
        }
    }
}
