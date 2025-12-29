namespace Chat.Persistence.Repositories
{
    public class MessageRepository : Repository<MessageModel, CoreDbContext>, IMessageRepository
    {
        public MessageRepository(CoreDbContext context) : base(context)
        {
        }
    }
}