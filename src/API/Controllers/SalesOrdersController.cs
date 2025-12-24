namespace API.Controllers;

[Route("sales-orders")]
[Authorize]
public class SalesOrdersController(
    ISalesOrdersService _service,
    IHubContext<OrderHub> _hubContext) : BaseApiController
{
    [HttpGet]
    [ValidateModel]
    public async Task<QueryResult<SalesOrderModel>> GetSalesOrdersWithPaging([FromQuery] QueryInfo queryInfo, [FromQuery] SalesOrderQuery orderQuery)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetSalesOrdersWithPaging(storeId, queryInfo, orderQuery);
    }
    [HttpGet("{customerId}/customers")]
    [AllowAnonymous]
    [ValidateModel]
    public async Task<QueryResult<SalesOrderOfCustomerModel>> GetSalesOrdersOfCustomerWithPagingAsync([FromRoute] Guid customerId, [FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetSalesOrdersOfCustomerWithPagingAsync(customerId, queryInfo);
    }
    [HttpGet("{id}")]
    public async Task<SalesOrderModel> GetSalesOrderById([FromRoute] Guid id)
    {
        return await _service.GetSalesOrderByIdAsync(id);
    }
    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> Create([FromBody] CreateSalesOrderRequest model)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
        //model.UserId = userId;
        model.StoreId = storeId;
        var result = await _service.CreateAsync(model, userId);

        if (result.Success)
        {
            await _hubContext.Clients.Group($"store-{storeId}").SendAsync("SalesOrderChanged");
            await _hubContext.Clients.Group("dashboard").SendAsync("SalesOrderChanged");
        }

        return result;
    }
    [HttpPatch("update-order-status")]
    public async Task<OperationResult> UpdateOrderStatus(UpdateOrderStatusModel model)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        model.StoreId = storeId;
        var result = await _service.UpdateOrderStatus(model);

        if (result.Success)
        {
            await _hubContext.Clients.Group($"store-{storeId}").SendAsync("SalesOrderChanged");
            await _hubContext.Clients.Group("dashboard").SendAsync("SalesOrderChanged");
        }
        return result;
    }
}
