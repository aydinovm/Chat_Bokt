namespace Chat.Persistence.Configurations
{
    public class ChatRequestConfiguration
        : IEntityTypeConfiguration<ChatRequestModel>
    {
        public void Configure(EntityTypeBuilder<ChatRequestModel> builder)
        {
            builder.ConfigureDefaults("ChatRequests");
            builder.ConfigureAudits();

            builder.HasOne(x => x.CreatedByUser)
                .WithMany(u => u.CreatedChats)
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AssignedToUser)
                .WithMany(u => u.AssignedChats)
                .HasForeignKey(x => x.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
