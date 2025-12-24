namespace Application;

public interface IProductsService
{
    Task<QueryResult<Product>> GetAllWithPagingAsync(QueryInfo queryInfo);
    Task<HashSet<string>> GetSkusAsync();
    Task<QueryResult<ProductWithPromotion>> GetProductsByStoreIdWithPagingAsync(Guid storeId, QueryInfo queryInfo);
    Task<QueryResult<Product>> GetProductsByCategoryGroupWithPaging(Guid storeId, string categoryGroup, QueryInfo queryInfo);
    Task<Product> GetByIdAsync(Guid id);
    Task<Product> GetBySkuAsync(string sku);
    Task<QueryResult<LowStockModel>> GetLowStockProductsAsync(Guid storeId, QueryInfo queryInfo);
    Task<OperationResult> CreateAsync(CreateProductModel model);
    Task<OperationResult> CreateAsync(List<CreateProductInfoModel> model);
    Task<OperationResult> UpdateAsync(Guid id, ProductUpdateModel model, Guid userId);
    Task<OperationResult> DeleteOrRestoreProductAsync(Guid id);
    Task<(bool IsValid, string? SuggestSku, string? Reason)> CheckSkuAsync(string sku);
}
