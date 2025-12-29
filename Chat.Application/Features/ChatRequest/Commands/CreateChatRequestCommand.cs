using Chat.Common.Helpers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Features
{
    public class CreateChatRequestCommand : BaseCommand, IRequest<Result<Unit>>
    {
        public Guid CreatedByUserId { get; set; }
        public Guid FromDepartmentId { get; set; }
        public Guid ToDepartmentId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        //public string Status { get; set; } = "Sent";
    }
}
