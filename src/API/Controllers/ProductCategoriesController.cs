namespace API.Controllers;

[Route("productcategories")]
[Authorize]
public class ProductCategoriesController
    (IProductCategoriesService _service) : BaseApiController
{
    [HttpGet]
    public async Task<QueryResult<ProductCategory>> GetProductCategoriesWithPaging([FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetProductCategoriesWithPagingAsync(queryInfo);
    }
    [HttpGet("group")]
    public async Task<List<ProductCategoriesIncludeProductGroupsModel>> GetProductCategoriesIncludeProductGroupsWithPaging()
    {
        return await _service.GetProductCategoriesIncludeProductGroupsWithPagingAsync();
    }
    [HttpGet("{id}")]
    public async Task<ProductCategory> GetProductCategoryById([FromRoute] Guid id)
    {
        return await _service.GetByIdAsync(id);
    }
    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> Create([FromBody] CreateOrUpdateProductCategoryModel model)
    {
        return await _service.CreateAsync(model);
    }
    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> Update([FromRoute] Guid id, [FromBody] CreateOrUpdateProductCategoryModel model)
    {
        return await _service.UpdateAsync(id, model);
    }
    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        return await _service.DeleteAsync(id);
    }
}
