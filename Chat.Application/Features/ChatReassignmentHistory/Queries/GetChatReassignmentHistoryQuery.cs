using Chat.Application.Responces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Features
{
    public class GetChatReassignmentHistoryQuery : IRequest<List<ChatReassignmentHistoryResponse>>
    {
        public Guid ChatRequestId { get; set; }
        public Guid UserId { get; set; } 
    }
}
