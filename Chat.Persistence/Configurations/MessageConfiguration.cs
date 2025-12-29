namespace Chat.Persistence.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<MessageModel>
    {
        public void Configure(EntityTypeBuilder<MessageModel> builder)
        {
            builder.ConfigureDefaults("Messages");
            builder.ConfigureAudits();
        }
    }
}
