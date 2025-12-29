using System.Linq.Expressions;

namespace Chat.Common.Persistence
{
    public interface IRepository<TEntity> where TEntity : BaseEntity, IEntity
    {
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task UpdateAsync(TEntity entity);
        Task UpdateRangeAsync(IEnumerable<TEntity> entities);
        Task RemoveAsync(TEntity entity);
        Task RemoveRangeAsync(IEnumerable<TEntity> entities);
        Task SaveChangesAsync();
        IQueryable<TEntity> FindAll();
        Task<TEntity> FindByIdAsync(Guid id, bool tracking = false);
        Task<TEntity> FindByIdAsyncIncludes(Guid id, bool tracking = false, params Expression<Func<TEntity, object>>[] includes);

    }
}
