using System.Linq.Expressions;

namespace AEIG.Domain.AppService.Services.Interfaces;

public interface IServiceBase<TEntity, TRequest, TResponse> where TEntity : class
{
    Task<TResponse> CreateAsync(TRequest request, bool isActive = true);
    Task<TResponse?> UpdateAsync(Guid navigationId, TRequest request);
    Task<TResponse> CreateAsync(TEntity entity, bool isActive = true);
    Task<List<TResponse>> CreateRangeAsync(List<TEntity> entities);
    Task<TResponse> UpdateAsync(TEntity entity);
    Task<TResponse?> UpdateAsync(Guid navigationId, Action<TEntity> applyChanges);
    Task UpdateRangeAsync(List<TEntity> entities);
    Task<TEntity1> UpdateDataAsync<TEntity1>(TEntity1 entity,
        params Expression<Func<TEntity1, IEnumerable<object>>>[] collections) where TEntity1 : class;
    Task<TResponse?> GetByIdAsync(Guid id);
    Task<List<TResponse>> GetAllAsync();
    Task<List<TResponse>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
    Task<AEIG.Domain.DynamicQueryResult<TResponse>> GetPagedAsync(AEIG.Domain.DynamicQuery query);
    IQueryable<TEntity> Query();
    Task DeleteAsync(TEntity entity);
    Task<bool> DeleteAsync(Guid navigationId);
    Task DeleteRangeAsync(List<TEntity> entities);
    Task DeleteRangeAsync(List<Guid> navigationIds);
}
