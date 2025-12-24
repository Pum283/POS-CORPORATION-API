namespace Infrastructure;

public interface IDapperRepositoryBase<TEntity> 
{
    Task<TEntity?> GetByIdAsync(Guid id);

    Task<IEnumerable<TEntity>> GetAllAsync(string tableName);

    Task<IEnumerable<TEntity>> QueryAsync(string sql, object? parameters = null);

    Task<TEntity?> QuerySingleAsync(string sql, object? parameters = null);

    Task<int> ExecuteAsync(string sql, object? parameters = null);

    Task<object> InsertAsync(TEntity entity);

    Task<bool> UpdateAsync( TEntity entity, string keyColumn = "Id");

    Task<bool> DeleteAsync(Guid id);
  
}
