namespace Infrastructure;

public interface IRepositoryBase<TEntity>
{
    Task<TEntity> AddAsync(TEntity entity);
    Task<bool> AddRangeAsync(IEnumerable<TEntity> entities);
    Task<bool> UpdateAsync(TEntity entity);
    void UpdateWithoutSave(TEntity entity);
    Task<int> DeleteByIdAsync(Guid id);
    void DeleteRangeAsync(IEnumerable<TEntity> entity);
    Task<IReadOnlyList<TEntity>> GetAllAsync();
    Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? condition);
    Task<TEntity> GetPurchaseOrderByIdAsync(Guid id);
    Task<TEntity> GetByIdAsync(Guid id, Expression<Func<TEntity, object>>[]? includes = null, CancellationToken cancellationToken = default);
    Task<List<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate);
    Task<List<TEntity>> GetListByConditionAsync(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TEntity>> GetByIdsAsync(List<Guid> ids, string fieldsName);
    Task<QueryResult<TEntity>> GetByPageAsync(QueryInfo queryInfo);

    Task<bool> CheckExistAsync(Expression<Func<TEntity, bool>> predicate);
    Task<bool> CheckExistByIdAsync(Guid id);
    IQueryable<TEntity> GetQueryable();
}
