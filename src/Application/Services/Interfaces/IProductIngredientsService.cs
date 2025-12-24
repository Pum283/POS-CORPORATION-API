namespace Application;

public interface IProductIngredientsService
{
    Task<QueryResult<ProductIngredient>> GetIngredientsForProductAsync(Guid productId, QueryInfo queryInfo);
    Task<OperationResult> AddIngredientToProductAsync(AddIngredientForProductModel model);
    Task<OperationResult> AddIngredientsToProductAsync(List<AddIngredientForProductModel> model);
    Task<OperationResult> UpdateIngredientInProductAsync(Guid id, UpdateProductIngredientModel model);
    //Task<OperationResult> DeductIngredientAsync(List<DeductIngredientModel> model);
    Task DeductIngredientAsync(List<DeductIngredientModel> models);
    Task<OperationResult> DeleteIngredientFromProductAsync(Guid id);
}
