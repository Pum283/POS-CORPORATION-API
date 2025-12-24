namespace Infrastructure;

public interface IOperationMetricsRepository : IRepositoryBase<OperationMetricEntity>
{
    Task<QueryResult<OperationMetricModel>> GetOperationMetricsAsync(QueryInfo queryInfo, OperationMetricQuery operationMetricQuery);
    Task<OperationMetricModel> GetOperationMetricByIdAsync(Guid id);
}
