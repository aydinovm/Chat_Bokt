using Chat.Common.Helpers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Features
    public class CloseChatCommand : BaseCommand, IRequest<Result<Unit>>
    {
        public Guid ChatRequestId { get; set; }
        public Guid ClosedByUserId { get; set; }
        public string? Reason { get; set; } // если хочешь
    }
}
