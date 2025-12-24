// Application/Services/Implements/ProductPriceHistoryService.cs
namespace Application;

[RegisterService(typeof(IProductPriceHistoryService))]
public class ProductPriceHistoryService(IUnitOfWork _unitOfWork) : IProductPriceHistoryService
{
    private IQueryable<ProductPriceHistoryModel> BaseQuery()
    {
        var historyQuery = _unitOfWork.Repository<ProductPriceHistoryEntity>()
            .GetQueryable()
            .OrderByDescending(x => x.CreateDate);

        var result = historyQuery
            .GroupJoin(
                _unitOfWork.Repository<UserEntity>().GetQueryable(),
                h => h.ChangedByUserId,
                u => u.Id,
                (h, users) => new { h, users })
            .SelectMany(
                x => x.users.DefaultIfEmpty(), // ← Left Join: nếu không có user → vẫn giữ history
                (x, u) => new { x.h, User = u })
            .Join(
                _unitOfWork.Repository<ProductEntity>().GetQueryable(),
                x => x.h.ProductId,
                p => p.Id,
                (x, p) => new { x.h, x.User, p })
            .GroupJoin(
                _unitOfWork.Repository<StoreEntity>().GetQueryable(),
                x => x.User != null ? x.User.StoreId : null,
                s => s.Id,
                (x, stores) => new { x.h, x.User, x.p, store = stores.FirstOrDefault() })
            .Select(x => new ProductPriceHistoryModel
            {
                Id = x.h.Id,
                ProductId = x.h.ProductId,
                ProductName = x.p.Name,
                ProductSKU = x.p.SKU,
                OldPrice = x.h.OldPrice,
                NewPrice = x.h.NewPrice,
                ChangedByUserId = x.h.ChangedByUserId,
                ChangedByUserName = x.User != null ? x.User.Name : null,  // ← null nếu user không tồn tại
                StoreId = x.User != null ? x.User.StoreId : null,
                StoreName = x.store != null ? x.store.Name : null,
                ChangedAt = x.h.CreateDate
            });

        return result;
    }

    private IQueryable<ProductPriceHistoryModel> ApplyDateFilter(
    IQueryable<ProductPriceHistoryModel> query,
    ProductPriceHistoryDateFilter? filter)
    {
        var from = filter?.FromDate ?? DateTime.Today;
        var to = filter?.ToDate ?? DateTime.Today.AddDays(1).AddTicks(-1);
        return query.Where(x => x.ChangedAt >= from && x.ChangedAt <= to);
    }

    //public async Task<QueryResult<ProductPriceHistoryModel>> GetAllAsync(QueryInfo queryInfo)
    //{
    //    var query = BaseQuery();

    //    var total = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;
    //    var data = await query.Skip(queryInfo.Skip).Take(queryInfo.Top).ToListAsync();

    //    return new QueryResult<ProductPriceHistoryModel> { Data = data, TotalCount = total };
    //}

    //public async Task<ProductPriceHistoryModel?> GetByIdAsync(Guid historyId)
    //    => await BaseQuery().FirstOrDefaultAsync(x => x.Id == historyId);

    //public async Task<QueryResult<ProductPriceHistoryModel>> GetByProductIdAsync(Guid productId, QueryInfo queryInfo)
    //{
    //    var query = BaseQuery().Where(x => x.ProductId == productId);

    //    var total = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;
    //    var data = await query.Skip(queryInfo.Skip).Take(queryInfo.Top).ToListAsync();

    //    return new QueryResult<ProductPriceHistoryModel> { Data = data, TotalCount = total };
    //}

    //public async Task<QueryResult<ProductPriceHistoryModel>> GetByUserIdAsync(Guid userId, QueryInfo queryInfo)
    //{
    //    var query = BaseQuery().Where(x => x.ChangedByUserId == userId);

    //    var total = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;
    //    var data = await query.Skip(queryInfo.Skip).Take(queryInfo.Top).ToListAsync();

    //    return new QueryResult<ProductPriceHistoryModel> { Data = data, TotalCount = total };
    //}

    //public async Task<QueryResult<ProductPriceHistoryModel>> GetByStoreIdAsync(Guid storeId, QueryInfo queryInfo)
    //{
    //    var query = BaseQuery().Where(x => x.StoreId == storeId);

    //    var total = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;
    //    var data = await query.Skip(queryInfo.Skip).Take(queryInfo.Top).ToListAsync();

    //    return new QueryResult<ProductPriceHistoryModel> { Data = data, TotalCount = total };
    //}

    public async Task<List<ProductPriceHistoryModel>> GetAllAsync(ProductPriceHistoryDateFilter? filter = null)
    {
        var query = ApplyDateFilter(BaseQuery(), filter);
        return await query.ToListAsync();
    }

    public async Task<ProductPriceHistoryModel?> GetByIdAsync(Guid historyId)
    => await BaseQuery().FirstOrDefaultAsync(x => x.Id == historyId);

    public async Task<List<ProductPriceHistoryModel>> GetByProductIdAsync(Guid productId, ProductPriceHistoryDateFilter? filter = null)
    {
        var query = ApplyDateFilter(BaseQuery().Where(x => x.ProductId == productId), filter);
        return await query.ToListAsync();
    }

    public async Task<List<ProductPriceHistoryModel>> GetByUserIdAsync(Guid userId, ProductPriceHistoryDateFilter? filter = null)
    {
        var query = ApplyDateFilter(BaseQuery().Where(x => x.ChangedByUserId == userId), filter);
        return await query.ToListAsync();
    }

    public async Task<List<ProductPriceHistoryModel>> GetByStoreIdAsync(Guid storeId, ProductPriceHistoryDateFilter? filter = null)
    {
        var query = ApplyDateFilter(BaseQuery().Where(x => x.StoreId == storeId), filter);
        return await query.ToListAsync();
    }
}