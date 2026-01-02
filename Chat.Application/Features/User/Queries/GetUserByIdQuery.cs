using Chat.Application.Responces;
using MediatR;

namespace Chat.Application.Features
{
    public class GetUserByIdQuery : IRequest<UserDetailResponse>
    {
        public Guid UserId { get; set; }
    }
}