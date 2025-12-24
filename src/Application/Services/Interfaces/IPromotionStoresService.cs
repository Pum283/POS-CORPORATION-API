namespace Application;

public interface IPromotionStoresService
{
    Task<PromotionStoreModel> GetPromotionsOfStoreWithPagingAsync(Guid storeId, QueryInfo queryInfo);
    Task<PromotionStoreModel> GetByIdAsync(Guid id);
    Task<OperationResult> UpdateAsync(Guid promotionId, UpdatePromotionStoreModel model);
    Task<OperationResult> DeleteAsync(Guid id);
}
