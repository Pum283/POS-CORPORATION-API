using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase<Guid>
{
    private readonly EfApplicationDbContext _context;

    public RepositoryBase(EfApplicationDbContext context)
    {
        _context = context;
    }
    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
        return entity;
    }

    public virtual async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _context.Set<TEntity>().AddRangeAsync(entities);
        return true;
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync()
    {
        var entities = await _context.Set<TEntity>().ToListAsync();
        return entities;
    }

    public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? condition)
    {
        if (condition == null)
        {
            return await _context.Set<TEntity>().CountAsync();
        }

        var entities = await _context.Set<TEntity>().ToListAsync();

        return entities.Count(condition.Compile());
    }

    public virtual async Task<TEntity> GetPurchaseOrderByIdAsync(Guid id)
    {
        var entity = await GetEntityByIdAsync(id);
        return entity;
    }

    public virtual async Task<TEntity> GetByIdAsync(Guid id, Expression<Func<TEntity, object>>[]? includes = null, CancellationToken cancellationToken = default)
    {
        if (includes == null)
        {
            return await GetEntityByIdAsync(id);
        }

        var query = _context.Set<TEntity>().AsQueryable();
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        var entity = await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
        return entity ?? throw new NotFoundException($"Entity with {id} was not found.");
    }

    public virtual async Task<IEnumerable<TEntity>> GetByIdsAsync(List<Guid> ids, string fieldsName)
    {
        var entities = await _context.Set<TEntity>().Where(domain => ids.Contains(EF.Property<Guid>(domain, fieldsName))).ToListAsync();
        return entities;
    }
    public async Task<List<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().Where(predicate).ToListAsync();
    }
    public virtual async Task<QueryResult<TEntity>> GetByPageAsync(QueryInfo queryInfo)
    {
        var query = _context.Set<TEntity>().AsQueryable();

        GenerateWhereString(query, queryInfo);

        if (!string.IsNullOrWhiteSpace(queryInfo.OrderBy))
        {
            query = queryInfo.OrderType == OrderType.Ascending
                ? query.OrderBy(e => EF.Property<object>(e, queryInfo.OrderBy))
                : query.OrderByDescending(e => EF.Property<object>(e, queryInfo.OrderBy));
        }

        int totalItems = 0;

        // Count number of record if required
        if (queryInfo.NeedTotalCount)
        {
            totalItems = await query.CountAsync();
        }

        var items = await query.Skip(queryInfo.Skip).Take(queryInfo.Top).ToListAsync();

        return new QueryResult<TEntity>
        {
            Data = items,
            TotalCount = totalItems
        };
    }
    public async Task<TEntity?> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
    }

    public async Task<List<TEntity>> GetListByConditionAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().Where(predicate).ToListAsync();
    }
    public virtual  Task<bool> UpdateAsync(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.FromResult(true);
    }
    public virtual void UpdateWithoutSave(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
    }

    public virtual async Task<int> DeleteByIdAsync(Guid id)
    {
        var entity = await GetEntityByIdAsync(id);
        _context.Set<TEntity>().Remove(entity);
        return 1;
    }


    public virtual async Task<bool> CheckExistAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().AnyAsync(predicate);
    }

    public virtual async Task<bool> CheckExistByIdAsync(Guid id)
    {
        var exists = await _context.Set<TEntity>().AnyAsync(
            e => EF.Property<Guid>(e, "Id") == id
        );
        return exists;
    }

    protected virtual async Task<TEntity> GetEntityByIdAsync(Guid id)
    {
        return await _context.Set<TEntity>().FindAsync(id) ?? throw new NotFoundException($"Entity with {id} was not found.");
    }

    protected virtual IQueryable<TEntity> GenerateWhereString(IQueryable<TEntity> query, QueryInfo queryInfo) => query;
    public IQueryable<TEntity> GetQueryable()
    {
        return _context.Set<TEntity>().AsQueryable();
    }

    public void DeleteRangeAsync(IEnumerable<TEntity> entities)
    {
        if (entities == null || !entities.Any())
            return;

        _context.Set<TEntity>().RemoveRange(entities);
    }
}