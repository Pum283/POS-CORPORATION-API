// File: Application/IFoodAppPricesService.cs
namespace Application;

public interface IFoodAppPricesService
{
    Task<OperationResult> CreateAsync(CreateFoodAppPriceModel model);
    Task<OperationResult> CreateBulkAsync(List<CreateFoodAppPriceModel> models);
    Task<QueryResult<FoodAppPriceModel>> GetAllWithPagingAsync(QueryInfo queryInfo);
    Task<FoodAppPriceModel> GetByIdAsync(Guid id);
    Task<OperationResult> UpdatePriceAsync(Guid id, UpdateFoodAppPriceModel model);
    Task<OperationResult> ToggleActiveAsync(Guid id);
    Task<QueryResult<FoodAppPriceModel>> GetByPlatformIdAsync(Guid platformId, QueryInfo queryInfo);
}