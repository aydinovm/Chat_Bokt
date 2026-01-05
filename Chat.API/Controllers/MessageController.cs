using Chat.Application.Features;
using Chat.Application.Responces.Message;
using Chat.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Chat.API.Controllers
{
    [ApiController]
    [Route("api/chat/{chatId:guid}/messages")]
    [Authorize]
    public class MessageController : BaseController
    {
        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // =========================
        // GET: api/chat/{chatId}/messages
        // =========================
        [HttpGet]
        public async Task<ActionResult<List<MessageResponse>>> GetMessages(Guid chatId)
        {
            var query = new GetChatMessagesQuery
            {
                ChatRequestId = chatId,
                UserId = CurrentUserId
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }

        // =========================
        // POST: api/chat/{chatId}/messages
        // =========================
        [HttpPost]
        public async Task<IActionResult> SendMessage(
            Guid chatId,
            [FromBody] SendMessageCommand command)
        {
            command.ChatRequestId = chatId;
            command.SenderUserId = CurrentUserId;

            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(new { messageId = result.Data });
        }

        // =========================
        // PUT: api/chat/{chatId}/messages/read
        // =========================
        [HttpPut("read")]
        public async Task<IActionResult> MarkAsRead(Guid chatId)
        {
            var command = new MarkMessagesAsReadCommand
            {
                ChatRequestId = chatId,
                UserId = CurrentUserId
            };

            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok();
        }

        // =========================
        // DELETE: api/chat/{chatId}/messages/{messageId}
        // =========================
        [HttpDelete("{messageId:guid}")]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            var command = new DeleteMessageCommand
            {
                MessageId = messageId,
                UserId = CurrentUserId
            };

            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return NoContent();
        }
    }
}
