using Domain.Models.PeriodLock;

namespace Application;

public interface IPeriodLocksService
{
    Task<string> GetPeriodLocksAsync(Guid storeId);
    Task<OperationResult> OpenPeriodLockAsync(CreatePeriodLockModel model);
    Task<OperationResult> ClosePeriodLockAsync(ClosePeriodLockModel model);
    Task<OperationResult> UpdateInventoryQuantityLockAsync(UpdateInventoryQuantityLockModel model);
    Task<OperationResult> UpdateIngredientInventoryQuantityLockAsync(UpdateIngredientInventoryQuantityLockModel model);
    Task<OperationResult> GetPeriodLocksByFilterAsync(FilterPeriodLockModel model);
}
