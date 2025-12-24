// File: Application/ILoyaltyPointConfigsService.cs
namespace Application;

public interface ILoyaltyPointConfigsService
{
    Task<OperationResult> CreateAsync(CreateLoyaltyPointConfigModel model, Guid createdBy);
    Task<QueryResult<LoyaltyPointConfigModel>> GetAllWithPagingAsync(QueryInfo queryInfo);
    Task<LoyaltyPointConfigModel> GetByIdAsync(Guid id);
    Task<OperationResult> UpdateAsync(Guid id, UpdateLoyaltyPointConfigModel model);
    Task<OperationResult> ToggleActiveAsync(Guid id);
    Task<LoyaltyPointConfigModel?> GetActiveConfigAsync();
}