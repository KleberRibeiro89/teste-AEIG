using System.Linq.Expressions;

namespace AEIG.Domain.Repositories;

public interface IRepositoryBase<T> where T : class
{
    Task<T> AddAsync(T entity, bool isActive = true);
    Task<List<T>> AddRangeAsync(List<T> entities);
    Task UpdateRangeAsync(List<T> entities);
    Task<T?> GetByIdAsync(Guid id);
    Task<List<T>> GetAllAsync();
    Task<T> UpdateAsync(T entity);
    Task<TEntity> UpdateDataAsync<TEntity>(TEntity entity,
        params Expression<Func<TEntity, IEnumerable<object>>>[] collections) where TEntity : class;
    Task DeleteAsync(T entity);
    Task DeleteRangeAsync(List<T> entities);
    Task DeleteRangeAsync(List<Guid> navigationIds);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
    Task<AEIG.Domain.DynamicQueryResult<T>> GetPagedAsync(AEIG.Domain.DynamicQuery query);
    IQueryable<T> Query();
}
