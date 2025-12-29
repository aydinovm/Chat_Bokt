namespace Chat.ModuleShared.Data
{
    public abstract class Repository<TEntity, TContext> : IRepository<TEntity>
            where TEntity : BaseEntity, IEntity
            where TContext : DbContext, new()
    {
        protected readonly TContext _context;

        public Repository(TContext context)
        {
            _context = context;
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public virtual void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            _context.SaveChanges();
        }

        public virtual void AddRange(IEnumerable<TEntity> entity)
        {
            _context.Set<TEntity>().AddRange(entity);
            _context.SaveChanges();
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _context.Set<TEntity>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            _context.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(TEntity entity)
        {
            _context.Attach(entity);
            _context.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            _context.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public IQueryable<TEntity> FindAll() => _context.Set<TEntity>().AsNoTracking();
        public async Task<TEntity> FindByIdAsync(Guid id, bool tracking)
            => tracking ? await _context.Set<TEntity>().FindAsync(id) : await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(_ => _.Id == id);

        public async Task<TEntity> FindByIdAsyncIncludes(Guid id, bool tracking, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (!tracking)
                query = query.AsNoTracking();

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }


    }
}
