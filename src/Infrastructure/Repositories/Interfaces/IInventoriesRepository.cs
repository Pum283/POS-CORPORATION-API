namespace Infrastructure;

public interface IInventoriesRepository : IRepositoryBase<InventoryEntity>
{
    Task<QueryResult<InventoryViewModel>> GetInventoriesWithPagingAsync(QueryInfo queryInfo);
    Task<QueryResult<InventoryViewModel>> GetInventoriesWithPagingByStoreIdAsync(Guid storeId, QueryInfo queryInfo);
    Task<QueryResult<StoreInventoryByProductCodeModel>> GetStoresByProductCodeAsync(string productCode, Guid currentStoreId, QueryInfo queryInfo);
}
