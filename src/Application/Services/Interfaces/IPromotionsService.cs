namespace Application;

public interface IPromotionsService
{
    Task<QueryResult<Promotion>> GetPromotionsAsync(QueryInfo queryInfo, GetPromotionsQuery promotionsQuery);
    Task<Promotion> GetPromotionByCodeAsync(string code);
    Task<Promotion> GetPromotionByIdAsync(Guid id);
    Task<List<PromotionOfProductModel>> GetPromotionsOfProductWithPagingAsync(Guid productId, Guid storeId);
    Task<PromotionModel> GetPromotionByProductIdAsync(Guid productId, Guid storeId);
    Task<PromotionModel> GetPromotionByProductGroupIdAsync(Guid productId, Guid productGroupId, Guid storeId);
    Task<OperationResult> CreateAsync(CreatePromotionStoreModel model);
    Task<OperationResult> UpdateAsync(Guid id, UpdatePromotionModel model);
    Task<OperationResult> BlockOrUnblockAsync(Guid id);
    Task<OperationResult> DeleteAsync(Guid id);
}
