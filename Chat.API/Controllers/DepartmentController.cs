using Chat.Application.Features;
using Chat.Application.Responces.Department;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Chat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<List<DepartmentDetailResponse>>> GetAll(
            [FromQuery] bool? isActive)
        {
            var query = new GetAllDepartmentsQuery
            {
                IsActive = isActive
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DepartmentDetailResponse>> GetById(Guid id)
        {
            var query = new GetDepartmentByIdQuery
            {
                DepartmentId = id
            };

            var result = await Mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentCommand command)
        {
            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(new { id = result.Data });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentCommand command)
        {

            command.DepartmentId = id;

            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteDepartmentCommand
            {
                DepartmentId = id
            };

            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return NoContent();
        }
    }
}
