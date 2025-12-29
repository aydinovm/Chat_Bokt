namespace Chat.Persistence.Configurations
{
    public class ChatRequestConfiguration : IEntityTypeConfiguration<ChatRequestModel>
    {
        public void Configure(EntityTypeBuilder<ChatRequestModel> builder)
        {
            builder.ConfigureDefaults("ChatRequests");
            builder.ConfigureAudits();
        }
    }
}
