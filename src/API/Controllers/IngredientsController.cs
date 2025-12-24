using Domain.Models.Ingredient;

namespace API.Controllers;

[Route("ingredients")]
[Authorize]
public class IngredientsController(IIngredientsService _service) : BaseApiController
{
    [HttpGet]
    public async Task<QueryResult<Ingredient>> GetIngredientWithPaging([FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetIngredientsWithPagingAsync(queryInfo);
    }
    [HttpGet("store")]
    public async Task<QueryResult<IngredientsInventoryReportModel>> GetStoreIngredientWithPaging
        ([FromQuery] QueryInfo queryInfo, [FromQuery] DateTime? date = null)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetStoreIngredientsPagedAsync(queryInfo, storeId, date);
    }
    [HttpGet("codes")]
    public async Task<HashSet<string>> GetCodes()
    {
        return await _service.GetCodesAsync();
    }
    [HttpGet("check-code/{code}")]
    public async Task<ActionResult<CheckCodeResult>> CheckIngredientCodeAsync(string code)
    {
        var result = await _service.CheckIngredientCodeAsync(code);

        var dto = new CheckCodeResult
        {
            IsValid = result.IsValid,
            SuggestCode = result.SuggestCode,
            Reason = result.Reason
        };

        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<Ingredient> GetIngredientById([FromRoute] Guid id)
    {
        return await _service.GetIngredientByIdAsync(id);
    }
    [HttpGet("codes/{code}")]
    public async Task<Ingredient> GetIngredientByCode([FromRoute] string code)
    {
        return await _service.GetIngredientByCodeAsync(code);
    }

    [HttpGet("low-stock")]
    public async Task<QueryResult<LowStockModel>> GetLowStockIngredients([FromQuery] QueryInfo queryInfo)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetLowStockIngredientsAsync(storeId, queryInfo);
    }
    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> CreateIngredient([FromBody] CreateOrUpdateIngredientModel model)
    {
        return await _service.CreateIngredientAsync(model);
    }
    [HttpPost("bulk")]
    [ValidateModel]
    public async Task<OperationResult> CreateIngredients([FromBody] List<CreateOrUpdateIngredientModel> model)
    {
        return await _service.CreateIngredientsAsync(model);
    }
    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> UpdateIngredient([FromRoute] Guid id, [FromBody] CreateOrUpdateIngredientModel model)
    {
        return await _service.UpdateIngredientAsync(id, model);
    }
    [HttpDelete("{id}")]
    public async Task<OperationResult> DeleteIngredient([FromRoute] Guid id)
    {
        return await _service.DeleteIngredientAsync(id);
    }
}
