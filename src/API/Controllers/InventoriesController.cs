namespace API.Controllers;

[Route("inventory")]
[Authorize]
public class InventoriesController(IInventoriesService _service) : BaseApiController
{
    [HttpGet]
    public async Task<QueryResult<InventoryViewModel>> GetInventoriesWithPaging([FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetInventoriesWithPagingAsync(queryInfo);
    }
    [HttpGet("{storeId}/store")]
    public async Task<QueryResult<InventoryViewModel>> GetInventoriesWithPagingByStoreId([FromRoute] Guid storeId, [FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetInventoriesWithPagingByStoreIdAsync(storeId, queryInfo);
    }

    [HttpGet("by-product/{productCode}")]
    public async Task<QueryResult<StoreInventoryByProductCodeModel>> GetStoresByProductCode([FromRoute] string productCode, [FromQuery] QueryInfo queryInfo)
    {
        var currentStoreId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetStoresByProductCodeAsync(productCode, currentStoreId, queryInfo);
    }

    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> Create([FromBody] CreateOrUpdateInventoryModel model)
    {
        return await _service.CreateAsync(model);
    }
    [HttpPatch("{id}")]
    public async Task<OperationResult> Update([FromRoute] Guid id, [FromBody] CreateOrUpdateInventoryModel model)
    {
        return await _service.UpdateAsync(id, model);
    }
}
