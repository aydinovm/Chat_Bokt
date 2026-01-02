using Chat.Application.Features;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers
{
    [Route("api/chat-requests")]
    [ApiController]
    public class ChatRequestController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> CreateChatRequest([FromBody] CreateChatRequestCommand command)
        {
            if (command == null)
                return BadRequest("Invalid chat request data.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            // ✅ Просто возвращаем Ok, т.к. ID нет
            return Ok(new { Message = "Chat request created successfully" });
        }

        // GET: api/chat-requests/department/{departmentId}
        [HttpGet("department/{departmentId}")]
        public async Task<IActionResult> GetChatsByDepartment([FromRoute] Guid departmentId)
        {
            var result = await Mediator.Send(new GetChatsQuery
            {
                DepartmentId = departmentId
            });

            return Ok(result);
        }

        // GET: api/chat-requests/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChatById([FromRoute] Guid id, [FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest("UserId is required.");

            var result = await Mediator.Send(new GetChatByIdQuery
            {
                ChatRequestId = id,
                UserId = userId
            });

            if (result == null)
                return NotFound("Chat request not found or access denied.");

            return Ok(result);
        }

        // PUT: api/chat-requests/{id}/resolve
        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> ResolveChat(
            [FromRoute] Guid id,
            [FromBody] ResolveChatCommand command)
        {
            if (command == null)
                return BadRequest("Invalid request data.");

            // ✅ Берём ID из route
            command.ChatRequestId = id;

            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(new { Message = "Chat resolved successfully" });
        }

        // PUT: api/chat-requests/{id}/reassign
        [HttpPut("{id}/reassign")]
        public async Task<IActionResult> ReassignChat(
            [FromRoute] Guid id,
            [FromBody] ReassignChatCommand command)
        {
            if (command == null)
                return BadRequest("Invalid request data.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ✅ Берём ID из route
            command.ChatRequestId = id;

            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(new { Message = "Chat reassigned successfully" });
        }
    }
}
