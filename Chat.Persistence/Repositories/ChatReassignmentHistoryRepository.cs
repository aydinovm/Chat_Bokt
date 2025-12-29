using Chat.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Persistence.Repositories
{
    public class ChatReassignmentHistoryRepository : Repository<ChatReassignmentHistoryModel, CoreDbContext>, IChatReassignmentHistoryRepositpry
    {
        public ChatReassignmentHistoryRepository(CoreDbContext context) : base(context)
        {
        }
    }
}
