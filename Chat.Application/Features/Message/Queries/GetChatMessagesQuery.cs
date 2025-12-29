using Chat.Application.Responces.Message;
using MediatR;

namespace Chat.Application.Features
{
    public class GetChatMessagesQuery : IRequest<List<MessageResponse>>
    {
        public Guid ChatRequestId { get; set; }
        public Guid UserId { get; set; } // Для проверки доступа
    }
}