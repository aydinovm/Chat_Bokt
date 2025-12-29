using Chat.Common.Helpers;
using MediatR;

namespace Chat.Application.Features
{
    public class DeleteMessageCommand : BaseCommand, IRequest<Result<Unit>>
    {
        public Guid MessageId { get; set; }
        public Guid UserId { get; set; } // Только отправитель может удалить
    }
}