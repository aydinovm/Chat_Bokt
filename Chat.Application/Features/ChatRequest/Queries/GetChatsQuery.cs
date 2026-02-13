using Chat.Application.Responces.ChatRequest;
using MediatR;

namespace Chat.Application.Features
{
    public class GetChatsQuery : IRequest<List<ChatRequestResponse>>
    {
        public Guid UserId { get; set; }

        // optional фильтры (можешь удалить если пока не надо)
        public bool? OnlyOpen { get; set; } // true => не Resolved/Closed
    }
}
