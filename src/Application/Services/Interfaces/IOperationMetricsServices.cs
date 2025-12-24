namespace Application;

public interface IOperationMetricsServices
{
    Task<QueryResult<OperationMetricModel>> GetOperationMetricsAsync(QueryInfo queryInfo, OperationMetricQuery operationMetricQuery);
    Task<OperationMetricModel> GetOperationMetricByIdAsync(Guid id);
    Task<OperationResult> CreateAsync(CreateOrUpdateOperationMetricModel model);
    Task<OperationResult> UpdateAsync(Guid id, CreateOrUpdateOperationMetricModel model);
    Task<OperationResult> DeleteAsync(Guid id);
}
