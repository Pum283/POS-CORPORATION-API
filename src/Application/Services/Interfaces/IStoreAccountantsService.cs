namespace Application;

public interface IStoreAccountantsService
{
    Task<QueryResult<StoreAccountListModel>> GetStoreAccountantsWithPagingAsync(QueryInfo queryInfo);
    Task<QueryResult<StoreAccountantModel>> GetStoreOfAccountantsWithPagingAsync(QueryInfo queryInfo, Guid accountantId);
    Task<List<StoreAccountantModel>?> GetStoresOfAccountantById(Guid accountantId);
    Task<OperationResult> CreateStoreAccountantAsync(CreateOrUpdateStoreAccountantModel model);
    Task<OperationResult> UpdateStoreAccountantAsync(Guid id, CreateOrUpdateStoreAccountantModel model);
    Task<OperationResult> DeleteStoreAccountantAsync(Guid id);
    Task<bool> HasStoreAccessAsync(Guid userId, Guid storeId);
}
