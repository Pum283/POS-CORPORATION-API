// File: API/Controllers/LoyaltyPointConfigsController.cs
namespace API.Controllers;

[Route("loyalty-point-configs")]
[Authorize] // hoặc
[Authorize(Roles = "Admin")] 
public class LoyaltyPointConfigsController(
    ILoyaltyPointConfigsService _service) : BaseApiController
{
    [HttpPost("create")]
    [ValidateModel]
    public async Task<OperationResult> Create([FromBody] CreateLoyaltyPointConfigModel model)
    {
        var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
        return await _service.CreateAsync(model, userId);
    }

    [HttpGet]
    public async Task<QueryResult<LoyaltyPointConfigModel>> GetWithPaging([FromQuery] QueryInfo queryInfo)
        => await _service.GetAllWithPagingAsync(queryInfo);

    [HttpGet("{id}")]
    public async Task<LoyaltyPointConfigModel> GetById([FromRoute] Guid id)
        => await _service.GetByIdAsync(id);

    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> Update([FromRoute] Guid id, [FromBody] UpdateLoyaltyPointConfigModel model)
        => await _service.UpdateAsync(id, model);

    [HttpPatch("toggle-active/{id}")]
    public async Task<OperationResult> ToggleActive([FromRoute] Guid id)
        => await _service.ToggleActiveAsync(id);

    // API/Controllers/LoyaltyPointConfigsController.cs

    [HttpGet("active")]
    public async Task<ActionResult<LoyaltyPointConfigModel>> GetActive()
    {
        var config = await _service.GetActiveConfigAsync();
        return config is null
            ? NotFound("Chưa có cấu hình tích điểm nào được kích hoạt.")
            : Ok(config);
    }
}