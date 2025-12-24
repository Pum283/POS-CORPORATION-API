namespace API.Controllers;

[Route("promotionstores")]
[Authorize]
public class PromotionStoresController(IPromotionStoresService _service) : BaseApiController
{
    [HttpGet("{storeId}/store")]
    public async Task<PromotionStoreModel> GetPromotionsOfStoreWithPaging([FromRoute] Guid storeId, [FromQuery] QueryInfo queryInfo)
    => await _service.GetPromotionsOfStoreWithPagingAsync(storeId, queryInfo);

    [HttpPatch("{promotionId}/promotion")]
    [ValidateModel]
    public async Task<OperationResult> Update(Guid promotionId, [FromBody] UpdatePromotionStoreModel model)
    {
        return await _service.UpdateAsync(promotionId, model);
    }

    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete(Guid id)
    {
        return await _service.DeleteAsync(id);
    }
}
