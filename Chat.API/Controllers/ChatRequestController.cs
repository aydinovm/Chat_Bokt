using Chat.Application.Features;
using Chat.Application.Responces.ChatRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers
{
    [Route("api/chat-requests")]
    [ApiController]
    [Authorize]
    public class ChatRequestController : BaseController
    {
        // =========================
        // GET: api/chat-requests/{id}
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
        // GET: api/chat-requests
        // =========================
        [HttpGet]
        public async Task<ActionResult<List<ChatRequestResponse>>> GetAll()
        {
            // ✅ Важно: не берём departmentId из JWT. Handler сам определит что отдавать по UserId.
            var query = new GetChatsQuery
            {
                UserId = CurrentUserId
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }

        // =========================
        // POST: api/chat-requests
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChatRequestCommand command)
        {
            if (command == null)
                return BadRequest("Invalid request body");

            command.CreatedByUserId = CurrentUserId;

            // ⚠️ Рекомендация:
            // Не доверяй FromDepartmentId с фронта.
            // Лучше в handler брать FromDepartmentId из пользователя CreatedByUserId.
            // (даже если пока не меняешь команду — в handler зафиксируй)

            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok();
        }

        // =========================
        // PUT: api/chat-requests/{id}/reassign
        // =========================
        [HttpPut("{id:guid}/reassign")]
        public async Task<IActionResult> Reassign(Guid id, [FromBody] ReassignChatCommand command)
        {
            if (command == null)
                return BadRequest("Invalid request body");

            command.ChatRequestId = id;
            command.ReassignedByUserId = CurrentUserId;

            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return NoContent();
        }

        // =========================
        // PUT: api/chat-requests/{id}/resolve
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

        // =========================
        // PUT: api/chat-requests/{id}/close
        // (только SuperAdmin проверит handler)
        // =========================
        [HttpPut("{id:guid}/close")]
        public async Task<IActionResult> Close(Guid id, [FromBody] CloseChatCommand? command)
        {
            command ??= new CloseChatCommand();

            command.ChatRequestId = id;
            command.ClosedByUserId = CurrentUserId;

            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok();
        }
    }
}
