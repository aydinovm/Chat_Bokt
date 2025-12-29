using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Domain.Persistence
{
    public class ChatReassignmentHistoryModel : BaseEntity, IEntity
    {
        public Guid ChatRequestId { get; set; }
        public Guid ReassignedByUserId { get; set; }
        public Guid? OldAssignedUserId { get; set; }
        public Guid NewAssignedUserId { get; set; }
        public string Reason { get; set; }
        public DateTime ReassignedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
