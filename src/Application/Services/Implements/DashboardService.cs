using Domain.Models;

namespace Application;

[RegisterService(typeof(IDashboardService))]
public class DashboardService(IDashboardRepository _repository) : IDashboardService
{
    public async Task<RevenueReportModel> GetRevenueReportAsync(QueryInfo queryInfo, GetRevenueReportQuery query)
    {
        return await _repository.GetRevenueReportAsync(queryInfo, query);
    }
    public async Task<DailyClosingReportModel> GetDailyReportAsync(/*QueryInfo queryInfo,*/ GetDailyClosingReportQuery query)
    {
        return await _repository.GetDailyReportAsync(/*queryInfo,*/ query);
    }

    public async Task<MainDiagramReportModel> GetMainDiagramReportAsync(QueryInfo queryInfo, MainDiagramReportQuery query)
    {
        return await _repository.GetMainDiagramReportAsync(queryInfo, query);
    }

    public async Task<InventoryReportModel> GetInventoryReportAsync(Guid storeId, QueryInfo queryInfo, InventoryDetailReportItemModel query)
    {
        return await _repository.GetInventoryReportAsync(storeId, queryInfo, query);
    }
    public async Task<QueryResult<ProductInventorySummaryModel>> GetInventoryProductSummariesAsync(Guid storeId, QueryInfo queryInfo, DateTime? date = null)
    {
        return await _repository.GetInventoryProductSummariesAsync(storeId, queryInfo, date);
    }
    public Task<InventoryReportSummaryModel> GetInventorySummaryReportAsync(Guid storeId, DateTime? date = null)
    {
        return _repository.GetInventorySummaryReportAsync(storeId, date);
    }
    //public async Task<QueryResult<AccountingInventoryModel>> GetProductsAccountingInventoryAsync(Guid storeId, GetAccountingInventoryQuery getProductsAccountingInventoryQuery, QueryInfo queryInfo)
    //{
    //    return await _repository.GetProductsAccountingInventoryAsync(storeId, getProductsAccountingInventoryQuery, queryInfo);
    //}
    public async Task<QueryResult<AccountingInventoryModel>> GetProductsAccountingInventoryAsync(Guid storeId, DateTime fromDate,
    DateTime toDate, QueryInfo queryInfo)
    {
        return await _repository.GetProductsAccountingInventoryAsync(storeId, fromDate, toDate, queryInfo);
    }

    //public async Task<QueryResult<AccountingIngredientInventoryModel>> GetIngredientsAccountingInventoryAsync(Guid storeId, GetAccountingInventoryQuery getProductsAccountingInventoryQuery, QueryInfo queryInfo)
    //{
    //    return await _repository.GetIngredientsAccountingInventoryAsync(storeId, getProductsAccountingInventoryQuery, queryInfo);

    //}

    public async Task<QueryResult<AccountingInventoryModel>> GetIngredientsAccountingInventoryAsync(Guid storeId, DateTime fromDate,
    DateTime toDate, QueryInfo queryInfo)
    {
        return await _repository.GetIngredientsAccountingInventoryAsync(storeId, fromDate, toDate, queryInfo);

    }

    public async Task<QueryResult<DocumentReportByTransType>> GetDocumentReportByTranTypeAsync(Guid storeId, DocumentReportByTransTypeQuery documentReportByTransTypeQuery, QueryInfo queryInfo)
    {
        return await _repository.GetDocumentReportByTranTypeAsync(storeId, documentReportByTransTypeQuery, queryInfo);
    }

    public async Task<List<TopStoreRevenueModel>> GetTopRevenseInStoresAsync(DashboardQuery query)
        => await _repository.GetTopRevenseInStoresAsync(query);
    public async Task<Dictionary<string, decimal>> GetTopPaymentMethodInStoresAsync(DashboardQuery query)
        => await _repository.GetTopPaymentMethodInStoresAsync(query);
    public IAsyncEnumerable<TopStoreRevenueModel> GetRevenseInStoresAsync(DashboardQuery query)
        => _repository.GetRevenseInStoresAsync(query);

    public async Task<SalesReportModel> GetSalesReportAsync(QueryInfo queryInfo, GetSalesReportQuery query)
    {
        return await _repository.GetSalesReportAsync(queryInfo, query);
    }

    public async Task<InventoryDetailReportModel> GetInventoryDetailReportAsync(QueryInfo queryInfo, GetInventoryDetailReportQuery query)
    {
        return await _repository.GetInventoryDetailReportAsync(queryInfo, query);
    }
    public async Task<SalesDetailReportModel> GetSalesDetailReportAsync(QueryInfo queryInfo, GetSalesDetailReportQuery query)
    {
        return await _repository.GetSalesDetailReportAsync(queryInfo, query);
    }
    public async Task<DailySalesReportModel> GetDailySalesReportAsync(/*QueryInfo queryInfo, */GetDailySalesReportQuery query)
    {
        return await _repository.GetDailySalesReportAsync(/*queryInfo,*/ query);
    }
}
