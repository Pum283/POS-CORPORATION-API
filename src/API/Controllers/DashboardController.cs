using Domain.Models;

namespace API.Controllers;

[Route("dashboards")]
[Authorize]
public class DashboardController(IDashboardService _service) : BaseApiController
{
    [HttpGet("revenue-report")]
    [ValidateModel]
    public async Task<RevenueReportModel> GetRevenueReport([FromQuery] QueryInfo queryInfo, [FromQuery] GetRevenueReportQuery query)
    {
        return await _service.GetRevenueReportAsync(queryInfo, query);
    }
    [HttpGet("closing-report")]
    [ValidateModel]
    public async Task<DailyClosingReportModel> GetDailyReport(/*[FromQuery] QueryInfo queryInfo,*/ [FromQuery] GetDailyClosingReportQuery query)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        query.StoreId = storeId;
        return await _service.GetDailyReportAsync(/*queryInfo, */query);
    }
    [HttpGet("main-diagram-report")]
    [ValidateModel]
    public async Task<MainDiagramReportModel> GetMainDiagramReport([FromQuery] QueryInfo queryInfo, [FromQuery] MainDiagramReportQuery query)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        query.StoreId = storeId;
        return await _service.GetMainDiagramReportAsync(queryInfo, query);
    }
    [HttpGet("revenue-report-order-off")]
    [ValidateModel]
    public async Task<MainDiagramReportModel> GetRevenueReportOrderOff([FromQuery] QueryInfo queryInfo, [FromQuery] MainDiagramReportQuery query)
    {
        return await _service.GetMainDiagramReportAsync(queryInfo, query);
    }
    [HttpGet("inventory-report")]
    [ValidateModel]
    public async Task<InventoryReportModel> GetInventotyReport([FromQuery] QueryInfo queryInfo, [FromQuery] InventoryDetailReportItemModel query)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetInventoryReportAsync(storeId, queryInfo, query);
    }
    [HttpGet("inventory-report-summary")]
    public async Task<InventoryReportSummaryModel> GetInventotyReportSumary()
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetInventorySummaryReportAsync(storeId);
    }

    [HttpGet("inventory-report-products")]
    public async Task<QueryResult<ProductInventorySummaryModel>> GetInventoryProductSummaries
        ([FromQuery] QueryInfo queryInfo, [FromQuery] DateTime? date = null)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetInventoryProductSummariesAsync(storeId, queryInfo, date);
    }

    //[HttpGet("product-accountant-inventory-report/{storeId}/store")]
    //[ValidateModel]
    //public async Task<QueryResult<AccountingInventoryModel>> GetProductsAccountingInventory
    //    ([FromRoute] Guid storeId, [FromQuery] GetAccountingInventoryQuery getProductsAccountingInventoryQuery, [FromQuery] QueryInfo queryInfo)
    //{
    //    return await _service.GetProductsAccountingInventoryAsync(storeId, getProductsAccountingInventoryQuery, queryInfo);
    //}

    [HttpGet("product-accountant-inventory-report/{storeId}/store")]
    [ValidateModel]
    public async Task<QueryResult<AccountingInventoryModel>> GetProductsAccountingInventory(
    [FromRoute] Guid storeId,
    [FromQuery] GetAccountingInventoryQuery query,
    [FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetProductsAccountingInventoryAsync(
            storeId,
            query.FromDate,
            query.ToDate,
            queryInfo
        );
    }

    //[HttpGet("ingredient-accountant-inventory-report/{storeId}/store")]
    //[ValidateModel]
    //public async Task<QueryResult<AccountingIngredientInventoryModel>> GetIngredientsAccountingInventory
    //    ([FromRoute] Guid storeId, [FromQuery] GetAccountingInventoryQuery getProductsAccountingInventoryQuery, [FromQuery] QueryInfo queryInfo)
    //{
    //    return await _service.GetIngredientsAccountingInventoryAsync(storeId, getProductsAccountingInventoryQuery, queryInfo);
    //}

    [HttpGet("ingredient-accountant-inventory-report/{storeId}/store")]
    [ValidateModel]
    public async Task<QueryResult<AccountingInventoryModel>> GetIngredientsAccountingInventory(
    [FromRoute] Guid storeId,
    [FromQuery] GetAccountingInventoryQuery query,
    [FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetIngredientsAccountingInventoryAsync(
            storeId,
            query.FromDate,
            query.ToDate,
            queryInfo
        );
    }

    [HttpGet("document-report-trantype/{storeId}/store")]
    [ValidateModel]
    public async Task<QueryResult<DocumentReportByTransType>> GetDocumentReportByType
        ([FromRoute] Guid storeId, [FromQuery] DocumentReportByTransTypeQuery documentReportByTransTypeQuery, [FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetDocumentReportByTranTypeAsync(storeId, documentReportByTransTypeQuery, queryInfo);
    }
    [HttpGet("top-store-revense")]
    public async Task<List<TopStoreRevenueModel>> GetTopRevenseInStores([FromQuery] DashboardQuery query)
        => await _service.GetTopRevenseInStoresAsync(query);
    [HttpGet("top-store-payment-method")]
    public async Task<Dictionary<string, decimal>> GetTopPaymentMethodInStores([FromQuery] DashboardQuery query)
        => await _service.GetTopPaymentMethodInStoresAsync(query);

    [HttpGet("revense")]
    public async Task GetRevenseInStores([FromQuery] DashboardQuery query)
    {
        Response.ContentType = "application/x-ndjson"; // NDJSON format
        //HttpContext.Response.Headers.Add("Cache-Control", "no-cache");
        //HttpContext.Response.Headers.Add("X-Accel-Buffering", "no"); // tránh proxy buffer

        await using var writer = new StreamWriter(Response.Body);

        await foreach (var item in _service.GetRevenseInStoresAsync(query))
        {
            var json = JsonSerializer.Serialize(item);
            await writer.WriteLineAsync(json);   // mỗi object một dòng
            await writer.FlushAsync();           // flush xuống client ngay
            await Task.Delay(500);              // delay 1 giây trước khi gửi object tiếp theo
        }
    }

    // ====================== ENDPOINT MỚI: BÁO CÁO BÁN HÀNG TIỀN ======================
    /// <summary>
    /// Báo cáo thống kê bán hàng chi tiết theo sản phẩm (có giảm giá phân bổ)
    /// - Lọc theo: FromDate, ToDate (bắt buộc), StoreId (tùy chọn), ProductGroupId (tùy chọn), ProductId (tùy chọn)
    /// - Hỗ trợ: phân trang, tìm kiếm, sắp xếp, lọc active theo QueryInfo
    /// </summary>
    [HttpGet("sales-report")]
    [ValidateModel]
    public async Task<SalesReportModel> GetSalesReport(
        [FromQuery] QueryInfo queryInfo,
        [FromQuery] GetSalesReportQuery query)
    {
        // Nếu muốn FromDate/ToDate optional (default toàn thời gian), bỏ comment 2 dòng dưới
        // query.FromDate = query.FromDate == default ? DateTime.MinValue : query.FromDate;
        // query.ToDate = query.ToDate == default ? DateTime.UtcNow : query.ToDate;

        return await _service.GetSalesReportAsync(queryInfo, query);
    }

    [HttpGet("inventory-detail-report")]
    [ValidateModel]
    public async Task<InventoryDetailReportModel> GetInventoryDetailReport(
    [FromQuery] QueryInfo queryInfo,
    [FromQuery] GetInventoryDetailReportQuery query)
    {
        return await _service.GetInventoryDetailReportAsync(queryInfo, query);
    }

    /// <summary>
    /// Báo cáo chi tiết bán hàng (chỉ orders ShippingType = None)
    /// - Lọc theo: FromDate, ToDate (bắt buộc), StoreId (tùy chọn)
    /// - Hỗ trợ: phân trang, tìm kiếm (SKU/Name), sắp xếp theo QueryInfo
    /// </summary>
    [HttpGet("sales-detail-report")]
    [ValidateModel]
    public async Task<SalesDetailReportModel> GetSalesDetailReport(
        [FromQuery] QueryInfo queryInfo,
        [FromQuery] GetSalesDetailReportQuery query)
    {
        // Nếu StoreId không truyền, lấy từ current user (tùy chọn, giống code gốc)
        // query.StoreId = query.StoreId ?? HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetSalesDetailReportAsync(queryInfo, query);
    }

    [HttpGet("daily-sales-report")]
    [ValidateModel]
    public async Task<DailySalesReportModel> GetDailySalesReport(
    //[FromQuery] QueryInfo queryInfo,
    [FromQuery] GetDailySalesReportQuery query)
    {
        // Lấy StoreName từ HttpContext hoặc query (giả sử dùng helper)
        //var storeName = HttpContextHelper.GetCurrentStoreName(HttpContext); // Hoặc query từ DB nếu cần
        var report = await _service.GetDailySalesReportAsync(/*queryInfo,*/ query);
        //report.StoreName = storeName; // Nếu cần set thêm
        return report;
    }

    [HttpGet("daily-sales-report-for-staff")]
    [ValidateModel]
    public async Task<DailySalesReportModel> GetDailySalesReporForStafft(
//[FromQuery] QueryInfo queryInfo,
    [FromQuery] GetDailySalesReportQuery query)
        {
            // Lấy StoreName từ HttpContext hoặc query (giả sử dùng helper)
            var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext); // Hoặc query từ DB nếu cần
            query.StoreId = storeId;
            var report = await _service.GetDailySalesReportAsync(/*queryInfo,*/ query);
            //report.StoreName = storeName; // Nếu cần set thêm
            return report;
        }
    }
