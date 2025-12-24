namespace Infrastructure;

public interface IPromotionStoresRepository : IRepositoryBase<PromotionStoreEntity>
{
    Task<PromotionStoreModel> GetPromotionsOfStorePagingAsync(Guid storeId, QueryInfo queryInfo);
    Task<PromotionStoreModel> GetByIdAsync(Guid id);
}
