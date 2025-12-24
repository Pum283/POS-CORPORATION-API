namespace Infrastructure;

public interface IProductGroupsRepository : IRepositoryBase<ProductGroupEntity>
{
    Task<QueryResult<ProductGroup>> GetProductGroupsWithPaging(QueryInfo queryInfo);
    Task<QueryResult<Product>> GetProductsByGroupIdWithPagingAsync(Guid storeId, Guid productGroupId, QueryInfo queryInfo);
    Task<QueryResult<ProductGroup>> GetProductGroupByCategoryIdWithPaging(Guid categoryId, QueryInfo queryInfo);
}
