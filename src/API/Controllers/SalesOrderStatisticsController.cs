// API/Controllers/SalesOrderStatisticsController.cs
namespace API.Controllers;

[Route("sales-order-statistics")]
[Authorize]
public class SalesOrderStatisticsController(
    ISalesOrdersService _service) : BaseApiController
{
    [HttpGet("order-count/store")]
    public async Task<object> GetOrderCountByStore(
        [FromQuery] Guid storeId,
        [FromQuery] ShippingType? platform = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
        => await _service.GetOrderCountByStoreAsync(storeId, platform, fromDate, toDate);

    [HttpGet("order-count/platform")]
    public async Task<Dictionary<string, object>> GetOrderCountByPlatform(
        [FromQuery] Guid storeId,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
        => await _service.GetOrderCountByPlatformAsync(storeId, fromDate, toDate);

    [HttpGet("order-count/date")]
    public async Task<Dictionary<string, object>> GetOrderCountByDate(
        [FromQuery] Guid storeId,
        [FromQuery] ShippingType? platform = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
        => await _service.GetOrderCountByDateAsync(storeId, platform, fromDate, toDate);

    [HttpGet("revenue/store")]
    public async Task<object> GetRevenueByStore(
        [FromQuery] Guid storeId,
        [FromQuery] ShippingType? platform = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
        => await _service.GetRevenueByStoreAsync(storeId, platform, fromDate, toDate);

    [HttpGet("revenue/platform")]
    public async Task<Dictionary<string, object>> GetRevenueByPlatform(
        [FromQuery] Guid storeId,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
        => await _service.GetRevenueByPlatformAsync(storeId, fromDate, toDate);

    [HttpGet("revenue/date")]
    public async Task<Dictionary<string, object>> GetRevenueByDate(
        [FromQuery] Guid storeId,
        [FromQuery] ShippingType? platform = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
        => await _service.GetRevenueByDateAsync(storeId, platform, fromDate, toDate);
}