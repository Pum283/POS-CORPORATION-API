using Domain.Models.PeriodLock;

namespace API.Controllers;

[Route("periodlocks")]
[Authorize]
public class PeriodLocksController(IPeriodLocksService _service) : BaseApiController
{
    [HttpGet("get-period-locks")]
    public async Task<string> GetPeriodLocks()
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetPeriodLocksAsync(storeId);
    }
    [HttpPost("open-lock")]
    public async Task<OperationResult> OpenPeriodLock(CreatePeriodLockModel model)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
        model.StoreId = storeId;
        model.UserId = userId;
        return await _service.OpenPeriodLockAsync(model);
    }
    [HttpPost("close-lock")]
    public async Task<OperationResult> ClosePeriodLock(ClosePeriodLockModel model)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
        model.StoreId = storeId;
        model.UserId = userId;
        return await _service.ClosePeriodLockAsync(model);
    }
    [HttpPatch("inventory-lock")]
    public async Task<OperationResult> UpdateInventoryQuantityLock(UpdateInventoryQuantityLockModel model)
    {
        return await _service.UpdateInventoryQuantityLockAsync(model);
    }
    [HttpPatch("ingredient-inventory-lock")]
    public async Task<OperationResult> UpdateIngredientInventoryQuantityLock(UpdateIngredientInventoryQuantityLockModel model)
    {
        return await _service.UpdateIngredientInventoryQuantityLockAsync(model);
    }

    [HttpPost("get-period-locks/current")]
    public async Task<OperationResult> GetPeriodLocksOfCurrentUser([FromBody] FilterPeriodLockDateModel model)
    {
        var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);

        var filter = new FilterPeriodLockModel
        {
            UserId = userId,
            StoreId = storeId,
            FromDate = model.FromDate,
            ToDate = model.ToDate
        };

        return await _service.GetPeriodLocksByFilterAsync(filter);
    }

    [HttpPost("get-period-locks-summary")]
    public async Task<OperationResult> GetPeriodLocksByFilter([FromBody] FilterPeriodLockModel model)
    {
        return await _service.GetPeriodLocksByFilterAsync(model);
        
    }
}
