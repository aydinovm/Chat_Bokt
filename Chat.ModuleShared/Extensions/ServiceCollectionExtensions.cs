namespace Chat.ModuleShared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CommonDbContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("ChatConnection")));

            services
                    .AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
