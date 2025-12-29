namespace Chat.Persistence.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CoreDbContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("ChatConnection")));

            services.AddScoped<IChatRequestRepository, ChatRequestRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IChatReassignmentHistoryRepositpry, ChatReassignmentHistoryRepository>();

            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
