namespace API.Controllers;

[Route("purchaseorders")]
[Authorize]
public class PurchaseOdersController(
    IPurchaseOrdersService _service) : BaseApiController
{
    [HttpGet]
    public async Task<QueryResult<PurchaseOrder>> GetPurchaseOrderWithPaging([FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetPurchaseOrderWithPagingAsync(queryInfo);
    }

    [HttpPost]
    public async Task<OperationResult> CreateOrder([FromBody] CreatePurchaseOrderModel model)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        var result = await _service.CreatePurchaseOrderAsync(storeId, model);
        return result;
    }
    [HttpPatch("{id}")]
    public async Task<OperationResult> UpdateOrderStatus([FromRoute] Guid id, [FromForm] OrderStatusType orderStatus)
    {
        return await _service.UpdateOrderStatusAsync(id, orderStatus);
    }
}
