namespace Infrastructure;

public interface IPromotionsRepository : IRepositoryBase<PromotionEntity>
{
    Task<QueryResult<Promotion>> GetPromotionsAsync(QueryInfo queryInfo, GetPromotionsQuery promotionsQuery);
    Task<List<PromotionOfProductModel>> GetPromotionsOfProductWithPagingAsync(Guid productId, Guid storeId);
    Task<Promotion> GetByIdAsync(Guid id);
    Task<PromotionModel> GetPromotionByProductIdAsync(Guid productId, Guid storeId);
    Task<PromotionModel> GetPromotionByProductGroupIdAsync(Guid productId, Guid productGroupId, Guid storeId);
}
