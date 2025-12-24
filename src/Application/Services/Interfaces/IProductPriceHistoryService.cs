// Application/Interfaces/IProductPriceHistoryService.cs
namespace Application;

public interface IProductPriceHistoryService
{
    //Task<QueryResult<ProductPriceHistoryModel>> GetAllAsync(QueryInfo queryInfo);
    //Task<ProductPriceHistoryModel?> GetByIdAsync(Guid historyId);
    //Task<QueryResult<ProductPriceHistoryModel>> GetByProductIdAsync(Guid productId, QueryInfo queryInfo);
    //Task<QueryResult<ProductPriceHistoryModel>> GetByUserIdAsync(Guid userId, QueryInfo queryInfo);
    //Task<QueryResult<ProductPriceHistoryModel>> GetByStoreIdAsync(Guid storeId, QueryInfo queryInfo);
    Task<List<ProductPriceHistoryModel>> GetAllAsync(ProductPriceHistoryDateFilter? filter = null);
    Task<ProductPriceHistoryModel?> GetByIdAsync(Guid historyId);
    Task<List<ProductPriceHistoryModel>> GetByProductIdAsync(Guid productId, ProductPriceHistoryDateFilter? filter = null);
    Task<List<ProductPriceHistoryModel>> GetByUserIdAsync(Guid userId, ProductPriceHistoryDateFilter? filter = null);
    Task<List<ProductPriceHistoryModel>> GetByStoreIdAsync(Guid storeId, ProductPriceHistoryDateFilter? filter = null);
}