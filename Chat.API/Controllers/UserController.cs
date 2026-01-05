using Chat.API.Services.Facades;
using Chat.Application.Features;
using Chat.Application.Responces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Chat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class UserController : BaseController
    {

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private bool IsDepartmentAdmin =>
            bool.TryParse(User.FindFirst("isDepartmentAdmin")?.Value, out var v) && v;

        private Guid? CurrentDepartmentId =>
            Guid.TryParse(User.FindFirst("departmentId")?.Value, out var id) ? id : null;

        [HttpGet("me")]
        public async Task<ActionResult<UserDetailResponse>> GetMyProfile()
        {
            var query = new GetUserByIdQuery { UserId = CurrentUserId };
            var result = await Mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserDetailResponse>> GetById(Guid id)
        {

            var query = new GetUserByIdQuery { UserId = id };
            var result = await Mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetAll([FromQuery] Guid? departmentId)
        {

            var query = new GetAllUsersQuery { DepartmentId = departmentId };
            var result = await Mediator.Send(query);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {

            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(new { id = result.Data });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
        {

            command.UserId = id;

            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {

            var command = new DeleteUserCommand { UserId = id };
            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return NoContent();
        }

        [HttpGet("check")]
        public IActionResult CheckAuth()
        {
            return Ok(new
            {
                UserId = CurrentUserId,
                DepartmentId = CurrentDepartmentId,
                IsDepartmentAdmin
            });
        }
    }
}
