namespace API.Controllers;

[Route("operationmetrics")]
[Authorize]
public class OperationMetricsController(IOperationMetricsServices _service) : BaseApiController
{
    [HttpGet]
    [ValidateModel]
    public async Task<QueryResult<OperationMetricModel>> GetOperationMetrics([FromQuery] QueryInfo queryInfo, [FromQuery] OperationMetricQuery operationMetricQuery)
    {
        return await _service.GetOperationMetricsAsync(queryInfo, operationMetricQuery);
    }

    [HttpGet("{id}")]
    public async Task<OperationMetricModel> GetOperationMetricById(Guid id)
    {
        return await _service.GetOperationMetricByIdAsync(id);
    }

    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> Create([FromBody] CreateOrUpdateOperationMetricModel model)
    {
        return await _service.CreateAsync(model);
    }
    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> Update([FromRoute] Guid id, [FromBody] CreateOrUpdateOperationMetricModel model)
    {
        return await _service.UpdateAsync(id, model);
    }
    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        return await _service.DeleteAsync(id);
    }
}
