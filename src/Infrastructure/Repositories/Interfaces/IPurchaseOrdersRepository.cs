namespace Infrastructure;

public interface IPurchaseOrdersRepository : IRepositoryBase<PurchaseOrderEntity>
{
    Task<QueryResult<PurchaseOrder>> GetPurchaseOrderWithPagingAsync(QueryInfo queryInfo);
    Task<PurchaseOrderEntity> GetPurchaseOrderWithDetailsByIdAsync(Guid id);
}
