namespace Infrastructure;

[RegisterService(typeof(IPromotionStoresRepository))]
public class PromotionStoresRepository(EfApplicationDbContext context) : RepositoryBase<PromotionStoreEntity>(context), IPromotionStoresRepository
{
    private readonly EfApplicationDbContext _context = context;

    public async Task<PromotionStoreModel> GetPromotionsOfStorePagingAsync(Guid storeId, QueryInfo queryInfo)
    {
        var store = await _context.Stores
            .AsNoTracking()
            .Where(s => s.Id == storeId)
            .Select(s => new { s.Id, s.Name })
            .FirstOrDefaultAsync();

        var promotionsQuery = from promotionStore in _context.PromotionStores.AsNoTracking()
                              where promotionStore.StoreId == storeId
                              join promotion in _context.Promotions.AsNoTracking()
                                  on promotionStore.PromotionId equals promotion.Id
                              orderby promotion.CreateDate descending
                              select new PromotionModel
                              {
                                  Id = promotion.Id,
                                  Code = promotion.Code,
                                  Name = promotion.Name,
                                  Type = promotion.Type.ToString(),
                                  Value = promotion.Value,
                                  StartDate = promotion.StartDate.ToString("dd-MM-yyyy"),
                                  EndDate = promotion.EndDate.ToString("dd-MM-yyyy"),
                                  UsageLimit = promotion.UsageLimit  ?? 0,
                                  UsageCount = promotion.UsageCount,
                                  IsActive = promotion.IsActive,
                                  ProductScope = promotion.ProductScope.ToString(),
                                  StoreScope = promotion.StoreScope.ToString()
                              };

        var promotions = await promotionsQuery
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        var totalCount = queryInfo.NeedTotalCount
            ? await promotionsQuery.CountAsync()
            : 0;

        return new PromotionStoreModel
        {
            StoreId = store != null ? store.Id : Guid.Empty,
            StoreName = store != null ? store.Name : "",
            Promotions = new QueryResult<PromotionModel>
            {
                Data = promotions,
                TotalCount = totalCount
            }
        };
    }


    public Task<PromotionStoreModel> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
