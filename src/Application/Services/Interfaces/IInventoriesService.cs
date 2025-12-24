namespace Application;

public interface IInventoriesService
{
    Task<QueryResult<InventoryViewModel>> GetInventoriesWithPagingAsync(QueryInfo queryInfo);
    Task<QueryResult<InventoryViewModel>> GetInventoriesWithPagingByStoreIdAsync(Guid storeId, QueryInfo queryInfo);
    Task<QueryResult<StoreInventoryByProductCodeModel>> GetStoresByProductCodeAsync(string productCode, Guid currentStoreId, QueryInfo queryInfo);
    Task<OperationResult> CreateAsync(CreateOrUpdateInventoryModel model);
    Task<OperationResult> UpdateAsync(Guid id, CreateOrUpdateInventoryModel model);
}
