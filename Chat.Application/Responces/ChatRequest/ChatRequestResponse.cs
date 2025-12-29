using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Responces.ChatRequest
{
    public class ChatRequestResponse : BaseResponce
    {
        public Guid CreatedByUserId { get; set; }
        public Guid FromDepartmentId { get; set; }
        public Guid ToDepartmentId { get; set; }
        public Guid? AssignedToUserId { get; set; }

        public string Status { get; set; }
        public string Title { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
