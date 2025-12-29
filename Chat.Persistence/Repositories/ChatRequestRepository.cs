namespace Chat.Persistence.Repositories
{
    public class ChatRequestRepository : Repository<ChatRequestModel, CoreDbContext>, IChatRequestRepository
    {
        public ChatRequestRepository(CoreDbContext context) : base(context)
        {
        }
    }

}
