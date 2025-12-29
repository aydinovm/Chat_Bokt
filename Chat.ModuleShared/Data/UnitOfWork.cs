namespace Chat.ModuleShared.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CommonDbContext _context;

        public UnitOfWork(CommonDbContext context) => _context = context;

        public async Task AsUnitAsync(Func<Task> func)
        {
            if (_context.Database.CurrentTransaction == null)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    await func();

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();

                    throw;
                }
            }
            else
            {
                await func();
            }
        }

        public async Task<T> AsUnitAsync<T>(Func<Task<T>> func)
        {
            if (_context.Database.CurrentTransaction == null)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var result = await func();

                    await transaction.CommitAsync();

                    return result;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();

                    throw;
                }
            }

            return await func();
        }

    }
}
