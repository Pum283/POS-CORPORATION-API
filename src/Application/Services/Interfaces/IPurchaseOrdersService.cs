namespace Application;

public interface IPurchaseOrdersService
{
    Task<QueryResult<PurchaseOrder>> GetPurchaseOrderWithPagingAsync(QueryInfo queryInfo);
    public Task<OperationResult> CreatePurchaseOrderAsync(Guid storeId, CreatePurchaseOrderModel model);
    public Task<OperationResult> UpdateOrderStatusAsync(Guid id, OrderStatusType orderStatus);
}
