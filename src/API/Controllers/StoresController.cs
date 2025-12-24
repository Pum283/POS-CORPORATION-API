namespace API.Controllers;

[Route("stores")]
public class StoresController(IStoresService _service) : BaseApiController
{
    [HttpGet]
    public async Task<QueryResult<Store>> GetStores([FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetStoresWithPagingAsync(queryInfo);
    }
    [HttpGet("without-account")]
    public async Task<QueryResult<Store>> GetStoresWithoutAccount([FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetStoresWithoutAccountAsync(queryInfo);
    }

    [HttpGet("{id}")]
    public async Task<Store> GetStoreById(Guid id)
    {
        return await _service.GetStoreByIdAsync(id);
    }

    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> CreateStore([FromBody] CreateOrUpdateStoreModel model)
    {
        return await _service.CreateStoreAsync(model);
    }
    [HttpPost("bulk")]
    [ValidateModel]
    public async Task<OperationResult> CreateStores([FromBody] List<CreateOrUpdateStoreModel> model)
    {
        return await _service.CreateStoresAsync(model);
    }
    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> UpdateStore([FromRoute] Guid id, [FromBody] CreateOrUpdateStoreModel model)
    {
        return await _service.UpdateStoreAsync(id, model);
    }
    [HttpDelete("{id}")]
    public async Task<OperationResult> DeleteStore([FromRoute] Guid id)
    {
        return await _service.DeleteStoreAsync(id);
    }
}
