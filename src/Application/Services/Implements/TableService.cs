namespace Application;

[RegisterService(typeof(ITableService))]
public class TableService(EfApplicationDbContext db) : ITableService
{
    public async Task<TableResponse> CreateAsync(TableCreateRequest request)
    {
        var entity = new TableEntity
        {
            Id = Guid.NewGuid(),
            StoreId = request.StoreId,
            TableNumber = request.TableNumber,
            IsActive = request.IsActive
        };
        db.Tables.Add(entity);
        await db.SaveChangesAsync();
        return new TableResponse
        {
            Id = entity.Id,
            StoreId = entity.StoreId,
            TableNumber = entity.TableNumber,
            IsActive = entity.IsActive
        };
    }

    public async Task<TableResponse?> GetByIdAsync(Guid id)
    {
        var entity = await db.Tables.FindAsync(id);
        if (entity == null) return null;
        return new TableResponse
        {
            Id = entity.Id,
            StoreId = entity.StoreId,
            TableNumber = entity.TableNumber,
            IsActive = entity.IsActive
        };
    }

    public async Task<List<TableResponse>> GetAllAsync(Guid? storeId = null)
    {
        var query = db.Tables.AsQueryable();
        if (storeId.HasValue)
            query = query.Where(x => x.StoreId == storeId.Value);

        return await query
            .Select(x => new TableResponse
            {
                Id = x.Id,
                StoreId = x.StoreId,
                TableNumber = x.TableNumber,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(TableUpdateRequest request)
    {
        var entity = await db.Tables.FindAsync(request.Id);
        if (entity == null) return false;
        entity.TableNumber = request.TableNumber;
        entity.IsActive = request.IsActive;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await db.Tables.FindAsync(id);
        if (entity == null) return false;
        db.Tables.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }
}
