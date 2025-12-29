using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Persistence.Configurations
{
    public class ChatReassignmentHistoryConfiguration : IEntityTypeConfiguration<ChatReassignmentHistoryModel>
    {
        public void Configure(EntityTypeBuilder<ChatReassignmentHistoryModel> builder)
        {
            builder.ConfigureDefaults("ChatReassignmentHistory");
            builder.ConfigureAudits();
        }
    }
}
