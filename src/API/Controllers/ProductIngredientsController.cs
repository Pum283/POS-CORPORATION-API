namespace API.Controllers;

[Route("product-ingredients")]
public class ProductIngredientsController(IProductIngredientsService _service) : BaseApiController
{
    [HttpGet("{productId}")]
    public async Task<QueryResult<ProductIngredient>> GetIngredientsForProduct([FromRoute]Guid productId, [FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetIngredientsForProductAsync(productId, queryInfo);
    }
    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> AddIngredientToProduct([FromBody] AddIngredientForProductModel model) 
    {
        return await _service.AddIngredientToProductAsync(model);
    }
    [HttpPost("bulk")]
    [ValidateModel]
    public async Task<OperationResult> AddIngredientsToProduct([FromBody] List<AddIngredientForProductModel> model)
    {
        return await _service.AddIngredientsToProductAsync(model);
    }
    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> UpdateIngredientInProduct([FromRoute] Guid id, [FromBody] UpdateProductIngredientModel model)
    {
        return await _service.UpdateIngredientInProductAsync(id, model);
    }
    //[HttpPatch("deduct")]
    //public async Task<OperationResult> DeductIngredient([FromBody] List<DeductIngredientModel> model)
    //{
    //    //var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
    //    //model.StoreId = storeId;
    //    return await _service.DeductIngredientAsync(model);
    //}
    [HttpDelete("{id}")]
    public async Task<OperationResult> DeleteIngredientFromProduct([FromRoute] Guid id)
    {
        return await _service.DeleteIngredientFromProductAsync(id);
    }
}
