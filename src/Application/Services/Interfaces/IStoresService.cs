namespace Application;

public interface IStoresService
{
    Task<QueryResult<Store>> GetStoresWithPagingAsync(QueryInfo queryInfo);
    Task<QueryResult<Store>> GetStoresWithoutAccountAsync(QueryInfo queryInfo);
    Task<Store> GetStoreByIdAsync(Guid id);
    Task<OperationResult> CreateStoreAsync(CreateOrUpdateStoreModel model);
    Task<OperationResult> CreateStoresAsync(List<CreateOrUpdateStoreModel> model);
    Task<OperationResult> UpdateStoreAsync(Guid id, CreateOrUpdateStoreModel model);
    Task<OperationResult> DeleteStoreAsync(Guid id);
    Task<List<Guid>> GetStoresByProductIdAsync(Guid id);
}
