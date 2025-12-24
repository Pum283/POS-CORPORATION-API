// File: API/Controllers/FoodAppPlatformsController.cs
namespace API.Controllers;

[Route("foodapp-platforms")]
[Authorize]
public class FoodAppPlatformsController(
    IFoodAppPlatformsService _service) : BaseApiController
{
    [HttpPost("create")]
    [ValidateModel]
    public async Task<OperationResult> Create([FromBody] CreateFoodAppPlatformModel model)
        => await _service.CreateAsync(model);

    [HttpGet]
    public async Task<QueryResult<FoodAppPlatformModel>> GetWithPaging([FromQuery] QueryInfo queryInfo)
        => await _service.GetAllWithPagingAsync(queryInfo);

    [HttpGet("{id}")]
    public async Task<FoodAppPlatformModel> GetById([FromRoute] Guid id)
        => await _service.GetByIdAsync(id);

    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> Update([FromRoute] Guid id, [FromBody] UpdateFoodAppPlatformModel model)
        => await _service.UpdateAsync(id, model);

    [HttpPatch("toggle-active/{id}")]
    public async Task<OperationResult> ToggleActive([FromRoute] Guid id)
        => await _service.ToggleActiveAsync(id);
}