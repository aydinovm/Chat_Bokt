namespace Chat.Persistence.Repositories
{
    public class UserRepository : Repository<UserModel, CoreDbContext>, IUserRepository
    {
        public UserRepository(CoreDbContext context) : base(context)
        {
        }
        public async Task<UserModel> FindByUserNameAsync(string userName)
          => await _context.Users.FirstOrDefaultAsync(x => x.Username == userName);
    }
}
