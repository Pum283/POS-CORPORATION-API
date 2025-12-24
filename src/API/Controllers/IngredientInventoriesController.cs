namespace API.Controllers;

[Route("ingredient-inventories")]
[Authorize]
public class IngredientInventoriesController(IIngredientInventoriesService _service) : BaseApiController
{
    [HttpPatch("{id}")]
    public async Task<OperationResult> UpdateIngredientInventory([FromRoute] Guid id, [FromBody] UpdateInventoryIngredientModel model)
    {
        return await _service.UpdateAsync(id, model);
    }
}
