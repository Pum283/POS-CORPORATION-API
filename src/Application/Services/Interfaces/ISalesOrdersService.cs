namespace Application;

public interface ISalesOrdersService
{
    Task<QueryResult<SalesOrderOfCustomerModel>> GetSalesOrdersOfCustomerWithPagingAsync(Guid customerId, QueryInfo queryInfo);
    Task<QueryResult<SalesOrderModel>> GetSalesOrdersWithPaging(Guid storeId, QueryInfo queryInfo, SalesOrderQuery orderQuery);
    Task<SalesOrderModel> GetSalesOrderByIdAsync(Guid id);
    Task<OperationResult> CreateAsync(CreateSalesOrderRequest model, Guid userId);
    Task<OperationResult> UpdateOrderStatus(UpdateOrderStatusModel model);
    // Application/ISalesOrdersService.cs
    Task<object> GetOrderCountByStoreAsync(Guid storeId, ShippingType? platform = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<Dictionary<string, object>> GetOrderCountByPlatformAsync(Guid storeId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<Dictionary<string, object>> GetOrderCountByDateAsync(Guid storeId, ShippingType? platform = null, DateTime? fromDate = null, DateTime? toDate = null);

    Task<object> GetRevenueByStoreAsync(Guid storeId, ShippingType? platform = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<Dictionary<string, object>> GetRevenueByPlatformAsync(Guid storeId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<Dictionary<string, object>> GetRevenueByDateAsync(Guid storeId, ShippingType? platform = null, DateTime? fromDate = null, DateTime? toDate = null);
}
