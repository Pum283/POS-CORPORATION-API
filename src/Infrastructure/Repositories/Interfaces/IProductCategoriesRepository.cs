namespace Infrastructure;

public interface IProductCategoriesRepository : IRepositoryBase<ProductCategoryEntity>
{
    Task<QueryResult<ProductCategory>> GetProductCategoriesWithPagingAsync(QueryInfo queryInfo);
    Task<List<ProductCategoriesIncludeProductGroupsModel>> GetProductCategoriesIncludeProductGroupsWithPagingAsync();
}
