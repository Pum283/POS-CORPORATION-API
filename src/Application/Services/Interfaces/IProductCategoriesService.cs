namespace Application;

public interface IProductCategoriesService
{
    Task<QueryResult<ProductCategory>> GetProductCategoriesWithPagingAsync(QueryInfo queryInfo);
    Task<List<ProductCategoriesIncludeProductGroupsModel>> GetProductCategoriesIncludeProductGroupsWithPagingAsync();
    Task<ProductCategory> GetByIdAsync(Guid id);
    Task<OperationResult> CreateAsync(CreateOrUpdateProductCategoryModel model);
    Task<OperationResult> UpdateAsync(Guid id, CreateOrUpdateProductCategoryModel model);
    Task<OperationResult> DeleteAsync(Guid id);
}
