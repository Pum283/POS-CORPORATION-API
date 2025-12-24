namespace Infrastructure;

public interface ISalesOrdersRepository : IRepositoryBase<SalesOrderEntity>
{
    Task<QueryResult<SalesOrderOfCustomerModel>> GetSalesOrdersOfCustomerWithPagingAsync(Guid customerId, QueryInfo queryInfo);
    Task<QueryResult<SalesOrderModel>> GetSalesOrdersWithPagingAsync(Guid storeId, QueryInfo queryInfo, SalesOrderQuery orderQuery);
    Task<SalesOrder> GetSalesOrderByIdForUpdateSOAsync(Guid id);
    Task<SalesOrderModel> GetSalesOrderByIdAsync(Guid id);
}
