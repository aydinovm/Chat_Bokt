namespace Chat.ModuleShared.Extensions
{
    public static class EntityTypeBuilderExtensions
    {
        public static EntityTypeBuilder<T> ConfigureAudits<T>(this EntityTypeBuilder<T> builder)
           where T : BaseEntity, IEntity
        {
            builder.Property(_ => _.IsDeleted).IsRequired();
            builder.HasIndex(_ => _.IsDeleted);
            return builder;
        }

        public static EntityTypeBuilder<T> ConfigureDefaults<T>(this EntityTypeBuilder<T> builder, string table) where T : BaseEntity
        {
            builder.ToTable(table);
            builder.Property(_ => _.Id);
            builder.HasKey(_ => _.Id);
            return builder;
        }
    }
}
