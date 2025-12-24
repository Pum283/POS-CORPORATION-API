namespace API.Controllers;

[Route("productgroups")]
[Authorize]
public class ProductGroupsController(IProductGroupsService _service) : BaseApiController
{
    [HttpGet]
    public async Task<QueryResult<ProductGroup>> GetProductGroupsWithPaging([FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetProductGroupsWithPaging(queryInfo);
    }
    [HttpGet("{productGroupId}/products")]
    public async Task<QueryResult<Product>> GetProductsByGroupWithPaging([FromRoute] Guid productGroupId, [FromQuery] QueryInfo queryInfo)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetProductsByGroupIdWithPagingAsync(storeId, productGroupId, queryInfo);
    }
    [HttpGet("{categoryId}/category")]
    public async Task<QueryResult<ProductGroup>> GetProductGroupByCategoryIdWithPaging([FromRoute] Guid categoryId, [FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetProductGroupByCategoryIdWithPaging(categoryId, queryInfo);
    }
    [HttpGet("{id}")]
    public async Task<ProductGroup> GetProductGroupById([FromRoute] Guid id)
    {
        return await _service.GetByIdAsync(id);
    }
    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> Create([FromBody] CreateOrUpdateProductGroupModel model)
    {
        return await _service.CreateAsync(model);
    }
    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> Update([FromRoute] Guid id, [FromBody] CreateOrUpdateProductGroupModel model)
    {
        return await _service.UpdateAsync(id, model);
    }
    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        return await _service.DeleteAsync(id);
    }
}
