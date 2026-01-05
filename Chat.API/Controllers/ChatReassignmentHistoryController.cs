using Chat.Application.Features;
using Chat.Application.Responces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers
{
    [ApiController]
    [Route("api/chats/{chatRequestId}/reassignment-history")]
    public class ChatReassignmentHistoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatReassignmentHistoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ChatReassignmentHistoryResponse>>> GetHistory(Guid chatRequestId)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

            var query = new GetChatReassignmentHistoryQuery
            {
                ChatRequestId = chatRequestId,
                UserId = userId
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }

    }
}
