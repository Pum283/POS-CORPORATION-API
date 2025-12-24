namespace Infrastructure;

public interface IStoresRepository : IRepositoryBase<StoreEntity>
{
    Task<QueryResult<Store>> GetStoresWithPagingAsync(QueryInfo queryInfo);
    Task<QueryResult<Store>> GetStoresWithoutAccountAsync(QueryInfo queryInfo);
    Task<Store> GetById(Guid id);
    Task<List<Guid>> GetStoresByProductIdAsync(Guid id);
}
