namespace Infrastructure;

public interface IStoreAccountantsRepository : IRepositoryBase<StoreAccountantEntity>
{
    Task<QueryResult<StoreAccountListModel>> GetStoreAccountantsWithPagingAsync(QueryInfo queryInfo);
    Task<QueryResult<StoreAccountantModel>> GetStoreOfAccountantsWithPagingAsync(QueryInfo queryInfo, Guid accountantId);
    Task<List<StoreAccountantModel>?> GetStoresOfAccountantByIdAsync(Guid accountantId);
}
