using Chat.Application.Features;
using Chat.Application.Responces.ChatRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Chat.API.Controllers
{
    [Route("api/chat-requests")]
    [ApiController]
    [Authorize]
    public class ChatRequestController : BaseController
    {
        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private Guid? CurrentDepartmentId =>
            Guid.TryParse(User.FindFirst("departmentId")?.Value, out var id) ? id : null;

        private bool IsDepartmentAdmin =>
            bool.TryParse(User.FindFirst("isDepartmentAdmin")?.Value, out var v) && v;

        // =========================
        // GET: api/chat/{id}
        // =========================
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ChatRequestDetailResponse>> GetById(Guid id)
        {
            var query = new GetChatByIdQuery
            {
                ChatRequestId = id,
                UserId = CurrentUserId
            };

            var result = await Mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // =========================
        // GET: api/chat
        // =========================
        [HttpGet]
        public async Task<ActionResult<List<ChatRequestResponse>>> GetAll()
        {
            if (CurrentDepartmentId == null)
                return Forbid();

            var query = new GetChatsQuery
            {
                DepartmentId = CurrentDepartmentId.Value
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }

        // =========================
        // POST: api/chat
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChatRequestCommand command)
        {
            command.CreatedByUserId = CurrentUserId;

            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok();
        }

        // =========================
        // PUT: api/chat/{id}/reassign
        // =========================
        [HttpPut("{id:guid}/reassign")]
        public async Task<IActionResult> Reassign(
            Guid id,
            [FromBody] ReassignChatCommand command)
        {
            if (!IsDepartmentAdmin)
                return Forbid();

            command.ChatRequestId = id;
            command.ReassignedByUserId = CurrentUserId;

            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok();
        }

        // =========================
        // PUT: api/chat/{id}/resolve
        // =========================
        [HttpPut("{id:guid}/resolve")]
        public async Task<IActionResult> Resolve(Guid id)
        {
            var command = new ResolveChatCommand
            {
                ChatRequestId = id,
                ResolvedByUserId = CurrentUserId
            };

            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok();
        }
    }
}

