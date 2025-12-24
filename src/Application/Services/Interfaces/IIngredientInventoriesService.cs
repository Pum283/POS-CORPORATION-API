namespace Application;

public interface IIngredientInventoriesService
{
    Task<OperationResult> UpdateAsync(Guid id, UpdateInventoryIngredientModel model);
}