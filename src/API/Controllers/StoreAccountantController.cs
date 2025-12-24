namespace API.Controllers;

[Route("storeaccountants")]
[Authorize]
public class StoreAccountantController(IStoreAccountantsService _service) : BaseApiController
{
    [HttpGet]
    public async Task<QueryResult<StoreAccountListModel>> GetStoreAccountantsWithPaging([FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetStoreAccountantsWithPagingAsync(queryInfo);
    }
    [HttpGet("accountant")]
    public async Task<QueryResult<StoreAccountantModel>> GetStoreOfAccountantsWithPaging([FromQuery] QueryInfo queryInfo)
    {
        var accountantId = HttpContextHelper.GetCurrentUserId(HttpContext);
        return await _service.GetStoreOfAccountantsWithPagingAsync(queryInfo, accountantId);
    }

    [HttpGet("{accountantId}")]
    public async Task<List<StoreAccountantModel>?> GetStoresOfAccountantById([FromRoute] Guid accountantId)
    {
        return await _service.GetStoresOfAccountantById(accountantId);
    }
    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> CreateStoreAccountant([FromBody] CreateOrUpdateStoreAccountantModel model)
    {
        return await _service.CreateStoreAccountantAsync(model);
    }
    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> UpdateStoreAccountant([FromRoute] Guid id, [FromBody] CreateOrUpdateStoreAccountantModel model)
    {
        return await _service.UpdateStoreAccountantAsync(id, model);
    }
    [HttpDelete("{id}")]
    public async Task<OperationResult> DeleteStoreAccountant([FromRoute] Guid id)
    {
        return await _service.DeleteStoreAccountantAsync(id);
    }
}
