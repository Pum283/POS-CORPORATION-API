using Domain.Models;

namespace Infrastructure;

public interface IDashboardRepository
{
    Task<RevenueReportModel> GetRevenueReportAsync(QueryInfo queryInfo, GetRevenueReportQuery query);
    Task<DailyClosingReportModel> GetDailyReportAsync(/*QueryInfo queryInfo,*/ GetDailyClosingReportQuery query);
    Task<MainDiagramReportModel> GetMainDiagramReportAsync(QueryInfo queryInfo, MainDiagramReportQuery query);
    Task<InventoryReportModel> GetInventoryReportAsync(Guid storeId, QueryInfo queryInfo, InventoryDetailReportItemModel query);
    Task<QueryResult<ProductInventorySummaryModel>> GetInventoryProductSummariesAsync(Guid storeId, QueryInfo queryInfo, DateTime? date = null);
    Task<InventoryReportSummaryModel> GetInventorySummaryReportAsync(Guid storeId, DateTime? date = null);
    //Task<QueryResult<AccountingInventoryModel>> GetProductsAccountingInventoryAsync(Guid storeId, GetAccountingInventoryQuery getProductsAccountingInventoryQuery, QueryInfo queryInfo);
    Task<QueryResult<AccountingInventoryModel>> GetProductsAccountingInventoryAsync(
    Guid storeId,
    DateTime fromDate,
    DateTime toDate,
    QueryInfo queryInfo);
    //Task<QueryResult<AccountingIngredientInventoryModel>> GetIngredientsAccountingInventoryAsync(Guid storeId, GetAccountingInventoryQuery getProductsAccountingInventoryQuery, QueryInfo queryInfo);
    Task<QueryResult<AccountingInventoryModel>> GetIngredientsAccountingInventoryAsync(
    Guid storeId,
    DateTime fromDate,
    DateTime toDate,
    QueryInfo queryInfo);
    Task<QueryResult<DocumentReportByTransType>> GetDocumentReportByTranTypeAsync
        (Guid storeId, DocumentReportByTransTypeQuery documentReportByTransTypeQuery, QueryInfo queryInfo);
    Task<List<TopStoreRevenueModel>> GetTopRevenseInStoresAsync(DashboardQuery query);
    Task<Dictionary<string, decimal>> GetTopPaymentMethodInStoresAsync(DashboardQuery query);
    IAsyncEnumerable<TopStoreRevenueModel> GetRevenseInStoresAsync(DashboardQuery query);
    Task<SalesReportModel> GetSalesReportAsync(QueryInfo queryInfo, GetSalesReportQuery query);
    Task<InventoryDetailReportModel> GetInventoryDetailReportAsync(QueryInfo queryInfo, GetInventoryDetailReportQuery query);
    Task<SalesDetailReportModel> GetSalesDetailReportAsync(QueryInfo queryInfo, GetSalesDetailReportQuery query);
    //Task<DailySalesReportModel> GetDailySalesReportAsync(/*QueryInfo queryInfo, */GetDailySalesReportQuery query);
    Task<DailySalesReportModel> GetDailySalesReportAsync(GetDailySalesReportQuery query);
}
