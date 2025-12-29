using Chat.Application.Responces.ChatRequest;
using MediatR;

namespace Chat.Application.Features
{
    public class GetChatByIdQuery : IRequest<ChatRequestDetailResponse>
    {
        public Guid ChatRequestId { get; set; }
        public Guid UserId { get; set; } // Для проверки доступа
    }
}