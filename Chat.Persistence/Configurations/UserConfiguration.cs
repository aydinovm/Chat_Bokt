namespace Chat.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserModel>
    {
        public void Configure(EntityTypeBuilder<UserModel> builder)
        {
            builder.ConfigureDefaults("Users");
            builder.ConfigureAudits();
        }
    }
}
