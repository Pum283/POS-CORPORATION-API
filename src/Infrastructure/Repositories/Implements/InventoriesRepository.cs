namespace Infrastructure;

[RegisterService(typeof(IInventoriesRepository))]
public class InventoriesRepository
    (EfApplicationDbContext context) 
    : RepositoryBase<InventoryEntity>(context), IInventoriesRepository
{
    private readonly EfApplicationDbContext _context = context;

    public async Task<QueryResult<InventoryViewModel>> GetInventoriesWithPagingAsync(QueryInfo queryInfo)
    {
        var query = _context.Inventories
            .AsNoTracking()
            .Where(x => !queryInfo.IsActive || x.Product!.IsActive == queryInfo.IsActive)
            .OrderByDescending(i => i.CreateDate)
            .Select(i => new InventoryViewModel
            {
                Id = i.Id,
                StoreId = i.StoreId,
                StoreName = i.Store != null ? i.Store.Name : null,
                ProductId = i.ProductId,
                ProductName = i.Product != null ? i.Product.Name : null,
                Quantity = i.Quantity
            });

        var pagedQuery = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        int totalCount = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalCount = await query.CountAsync();
        }
        return new QueryResult<InventoryViewModel>
        {
            Data = pagedQuery,
            TotalCount = totalCount
        };
    }

    public async Task<QueryResult<InventoryViewModel>> GetInventoriesWithPagingByStoreIdAsync(Guid storeId, QueryInfo queryInfo)
    {
        var query = _context.Inventories
            .AsNoTracking()
            .Where(i => i.StoreId == storeId &&
                        (!queryInfo.IsActive || i.Product!.IsActive == queryInfo.IsActive))
            .OrderByDescending(i => i.CreateDate)
            .Select(i => new InventoryViewModel
            {
                Id = i.Id,
                StoreId = i.StoreId,
                StoreName = i.Store != null ? i.Store.Name : null,
                ProductId = i.ProductId,
                ProductName = i.Product != null ? i.Product.Name : null,
                Quantity = i.Quantity
            });

        var pagedQuery = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        int totalCount = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalCount = await query.CountAsync();
        }
        return new QueryResult<InventoryViewModel>
        {
            Data = pagedQuery,
            TotalCount = totalCount
        };
    }

    public async Task<QueryResult<StoreInventoryByProductCodeModel>> GetStoresByProductCodeAsync(
        string productCode, Guid currentStoreId, QueryInfo queryInfo)
    {
        var inventoriesQuery = _context.Inventories
            .AsNoTracking()
            .Where(i => i.Product != null && i.Product.SKU == productCode && i.StoreId != currentStoreId)
            .Select(i => new StoreInventoryByProductCodeModel
            {
                StoreId = i.StoreId,
                StoreName = i.Store != null ? i.Store.Name : string.Empty, 
                Address = i.Store != null && i.Store.Address != null ? i.Store.Address : string.Empty,
                Quantity = i.Quantity,
                Status = i.Quantity >= 30 ? "Nhiều hàng"
                         : i.Quantity >= 15 ? "Trung bình"
                         : i.Quantity >= 5 ? "Thấp"
                         : "Rất thấp"
            });

        var ingredientInventoriesQuery = _context.IngredientInventories
            .AsNoTracking()
            .Where(i => i.Ingredient != null && i.Ingredient.Code == productCode && i.StoreId != currentStoreId)
            .Select(i => new StoreInventoryByProductCodeModel
            {
                StoreId = i.StoreId,
                StoreName = i.Store != null ? i.Store.Name : string.Empty,
                Address = i.Store != null ? i.Store.Address : string.Empty,
                Quantity = i.Quantity,
                Status = i.Quantity >= 30 ? "Nhiều hàng"
                         : i.Quantity >= 15 ? "Trung bình"
                         : i.Quantity >= 5 ? "Thấp"
                         : "Rất thấp"
            });

        var combinedQuery = inventoriesQuery.Union(ingredientInventoriesQuery);

        int totalCount = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalCount = await combinedQuery.CountAsync();
        }

        var data = await combinedQuery
            .OrderByDescending(i => i.Quantity)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        return new QueryResult<StoreInventoryByProductCodeModel>
        {
            Data = data,
            TotalCount = totalCount
        };
    }

}
