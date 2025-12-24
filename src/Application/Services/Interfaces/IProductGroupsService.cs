namespace Application;

public interface IProductGroupsService
{
    Task<QueryResult<ProductGroup>> GetProductGroupsWithPaging(QueryInfo queryInfo);
    Task<QueryResult<Product>> GetProductsByGroupIdWithPagingAsync(Guid storeId, Guid productGroupId, QueryInfo queryInfo);
    Task<QueryResult<ProductGroup>> GetProductGroupByCategoryIdWithPaging(Guid categoryId, QueryInfo queryInfo);
    Task<ProductGroup> GetByIdAsync(Guid id);
    Task<OperationResult> CreateAsync(CreateOrUpdateProductGroupModel model);
    Task<OperationResult> UpdateAsync(Guid id, CreateOrUpdateProductGroupModel model);
    Task<OperationResult> DeleteAsync(Guid id);
}
