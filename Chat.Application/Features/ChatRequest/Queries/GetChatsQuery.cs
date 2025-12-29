using Chat.Application.Responces.ChatRequest;
using MediatR;


namespace Chat.Application.Features
{
    public class GetChatsQuery : IRequest<List<ChatRequestResponse>>
    {
        public Guid DepartmentId { get; set; }
    }
}
