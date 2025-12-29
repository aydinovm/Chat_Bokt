namespace Chat.ModuleShared
{
    public class CommonDbContext : ProjectDbContext<CommonDbContext>
    {
        public CommonDbContext()
        {

        }
        public CommonDbContext(DbContextOptions<CommonDbContext> options) : base(options)
        {

        }
    }
}
