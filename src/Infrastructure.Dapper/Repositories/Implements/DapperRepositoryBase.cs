namespace Infrastructure.Dapper;

public class DapperRepositoryBase<TEntity> : IDapperRepositoryBase<TEntity>
    where TEntity : class
{
    private readonly DapperApplicationDbContext _context;
    public DapperRepositoryBase(DapperApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        var tableName = typeof(TEntity).GetDbTableName();
        var sql = $"SELECT * FROM {tableName} WHERE Id = @Id";
        return await _context.Connection!.QueryFirstOrDefaultAsync<TEntity>(sql, new { Id = id });
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(string tableName)
    {
        var sql = $"SELECT * FROM {tableName}";
        return await _context.Connection!.QueryAsync<TEntity>(sql);
    }

    public async Task<IEnumerable<TEntity>> QueryAsync(string sql, object? parameters = null)
    {
        return await _context.Connection!.QueryAsync<TEntity>(sql, parameters);
    }

    public async Task<TEntity?> QuerySingleAsync(string sql, object? parameters = null)
    {
        return await _context.Connection!.QueryFirstOrDefaultAsync<TEntity>(sql, parameters);
    }

    public async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        return await _context.Connection!.ExecuteAsync(sql, parameters);
    }

    public async Task<object> InsertAsync(TEntity entity)
    {
        var props = typeof(TEntity).GetProperties().Where(p => p.Name != "Id").ToList();
        var tableName = typeof(TEntity).GetDbTableName();
        var columns = string.Join(", ", props.Select(p => p.Name));
        var parameters = string.Join(", ", props.Select(p => "@" + p.Name));

        var sql = $@"INSERT INTO {tableName} ({columns})
                         VALUES ({parameters}); 
                         SELECT CAST(SCOPE_IDENTITY() as int);";

        var paramObject = new DynamicParameters();
        foreach (var prop in props)
        {
            paramObject.Add("@" + prop.Name, prop.GetValue(entity));
        }

        return await _context.Connection!.ExecuteScalarAsync<int>(sql, paramObject);
    }

    public async Task<bool> UpdateAsync(TEntity entity, string keyColumn)
    {
        var props = typeof(TEntity).GetProperties().Where(p => p.Name != keyColumn).ToList();
        var tableName = typeof(TEntity).GetDbTableName();
        var setClause = string.Join(", ", props.Select(p => $"{p.Name} = @{p.Name}"));
        var sql = $"UPDATE {tableName} SET {setClause} WHERE {keyColumn} = @{keyColumn}";

        var paramObject = new DynamicParameters();
        foreach (var prop in typeof(TEntity).GetProperties())
        {
            paramObject.Add("@" + prop.Name, prop.GetValue(entity));
        }

        var affected = await _context.Connection!.ExecuteAsync(sql, paramObject);
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var tableName = typeof(TEntity).GetDbTableName();
        var sql = $"DELETE FROM {tableName} WHERE Id = @Id";
        var affected = await _context.Connection!.ExecuteAsync(sql, new { Id = id });
        return affected > 0;
    }
}
