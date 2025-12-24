// File: API/Controllers/FoodAppPricesController.cs
namespace API.Controllers;

[Route("foodapp-prices")]
[Authorize]
public class FoodAppPricesController(
    IFoodAppPricesService _service) : BaseApiController
{
    [HttpPost("create")]
    [ValidateModel]
    public async Task<OperationResult> Create([FromBody] CreateFoodAppPriceModel model)
        => await _service.CreateAsync(model);

    [HttpPost("bulk")]
    //[ValidateModel]
    public async Task<OperationResult> CreateBulk([FromBody] List<CreateFoodAppPriceModel> models)
        => await _service.CreateBulkAsync(models);

    [HttpGet]
    public async Task<QueryResult<FoodAppPriceModel>> GetWithPaging([FromQuery] QueryInfo queryInfo)
        => await _service.GetAllWithPagingAsync(queryInfo);

    [HttpGet("{id}")]
    public async Task<FoodAppPriceModel> GetById([FromRoute] Guid id)
        => await _service.GetByIdAsync(id);

    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> Update([FromRoute] Guid id, [FromBody] UpdateFoodAppPriceModel model)
        => await _service.UpdatePriceAsync(id, model);

    [HttpPatch("toggle-active/{id}")]
    public async Task<OperationResult> ToggleActive([FromRoute] Guid id)
        => await _service.ToggleActiveAsync(id);

    [HttpGet("platform/{platformId}")]
    public async Task<QueryResult<FoodAppPriceModel>> GetByPlatformId(
        Guid platformId,
        [FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetByPlatformIdAsync(platformId, queryInfo);
    }
}