namespace Infrastructure;

public interface IProductsRepository : IRepositoryBase<ProductEntity>
{
    Task<QueryResult<Product>> GetAllWithPagingAsync(QueryInfo queryInfo);
    Task<QueryResult<ProductWithPromotion>> GetProductsByStoreIdWithPaging(Guid storeId, QueryInfo queryInfo);
    Task<QueryResult<Product>> GetProductsByCategoryGroupWithPaging(Guid storeId, string categoryGroup, QueryInfo queryInfo);
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product> GetBySkuAsync(string sku);
    Task<Product> GetProductForSalesOrderAsync(Guid id);
    Task<HashSet<string>> GetProductSkusAsync();
    Task<HashSet<Guid>> GetProductIdsAsync();
    Task<QueryResult<LowStockModel>> GetLowStockProductsAsync(Guid storeId, QueryInfo queryInfo);
    Task<Product?> GetProductForStaffOrderAsync(Guid productId, Guid storeId);
}
