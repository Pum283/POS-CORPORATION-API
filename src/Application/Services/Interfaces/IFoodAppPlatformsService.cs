// File: Application/IFoodAppPlatformsService.cs
namespace Application;

public interface IFoodAppPlatformsService
{
    Task<OperationResult> CreateAsync(CreateFoodAppPlatformModel model);
    Task<QueryResult<FoodAppPlatformModel>> GetAllWithPagingAsync(QueryInfo queryInfo);
    Task<FoodAppPlatformModel> GetByIdAsync(Guid id);
    Task<OperationResult> UpdateAsync(Guid id, UpdateFoodAppPlatformModel model);
    Task<OperationResult> ToggleActiveAsync(Guid id);
}