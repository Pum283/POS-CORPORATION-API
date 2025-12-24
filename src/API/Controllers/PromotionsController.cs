namespace API.Controllers;

[Route("promotions")]
[Authorize]
public class PromotionsController(IPromotionsService _service) : BaseApiController
{
    [HttpGet]
    public async Task<QueryResult<Promotion>> GetPromotions([FromQuery] QueryInfo queryInfo, [FromQuery] GetPromotionsQuery promotionQuery)
    {
        return await _service.GetPromotionsAsync(queryInfo, promotionQuery);
    }
    [HttpGet("{id}")]
    public async Task<Promotion> GetPromotionByCode([FromRoute] Guid id)
    {
        return await _service.GetPromotionByIdAsync(id);
    }
    [HttpGet("code/{code}")]
    public async Task<Promotion> GetPromotionByCode([FromRoute] string code)
    {
        return await _service.GetPromotionByCodeAsync(code);
    }
    [HttpGet("{productId}/product")]
    public async Task<List<PromotionOfProductModel>> GetPromotionsOfProductWithPaging([FromRoute] Guid productId)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetPromotionsOfProductWithPagingAsync(productId, storeId);
    }

    [HttpGet("top/{productId}/product")]
    public async Task<PromotionModel> GetPromotionByProductId([FromRoute] Guid productId)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetPromotionByProductIdAsync(productId, storeId);
    }
    [HttpGet("top/{productId}/product/{productGroupId}/productgroup")]
    public async Task<PromotionModel> GetPromotionByProductGroupId([FromRoute] Guid productId, [FromRoute] Guid productGroupId)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetPromotionByProductGroupIdAsync(productId, productGroupId, storeId);
    }
    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> Create([FromBody] CreatePromotionStoreModel model)
    {
        return await _service.CreateAsync(model);
    }
    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> Update(Guid id, [FromBody] UpdatePromotionModel model)
    {
        return await _service.UpdateAsync(id, model);
    }
    [HttpPatch("{id}/block")]
    [ValidateModel]
    public async Task<OperationResult> BlockOrUnblock(Guid id)
    {
        return await _service.BlockOrUnblockAsync(id);
    }
}
