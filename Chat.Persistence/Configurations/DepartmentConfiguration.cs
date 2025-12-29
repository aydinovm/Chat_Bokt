namespace Chat.Persistence.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<DepartmentModel>
    {
        public void Configure(EntityTypeBuilder<DepartmentModel> builder)
        {
            builder.ConfigureDefaults("Departments");
            builder.ConfigureAudits();
        }
    }

}
