using Domain.Models.Product;

namespace API.Controllers;

[Route("products")]
[Authorize]
public class ProductsController(
    IProductsService _service,
    IStoresService _storesService,
    IHubContext<ProductHub> _hubContext
    //IValidator<CreateProductInfoModel> _validator,
    //IMapper _mapper
    ) : BaseApiController
{
    [HttpGet]
    public async Task<QueryResult<Product>> GetProductsWithPaging([FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetAllWithPagingAsync(queryInfo);
    }
    [HttpGet("skus")]
    public async Task<HashSet<string>> GetSkus()
    {
        return await _service.GetSkusAsync();
    }
    [HttpGet("store")]
    public async Task<QueryResult<ProductWithPromotion>> GetProductsByStoreIdWithPaging([FromQuery] QueryInfo queryInfo)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetProductsByStoreIdWithPagingAsync(storeId, queryInfo);
    }

    [HttpGet("category-group")]
    public async Task<QueryResult<Product>> GetProductsByCategoryGroupWithPaging([FromQuery] string categoryGroup, [FromQuery] QueryInfo queryInfo)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetProductsByCategoryGroupWithPaging(storeId, categoryGroup, queryInfo);
    }

    [HttpGet("{id}")]
    public async Task<Product> GetProductById([FromRoute] Guid id)
    {
        return await _service.GetByIdAsync(id);
    }

    [HttpGet("sku/{sku}")]
    public async Task<Product> GetProductBySku([FromRoute] string sku)
    {
        return await _service.GetBySkuAsync(sku);
    }
    [HttpGet("check-sku/{sku}")]
    public async Task<ActionResult<CheckSkuResult>> CheckSkuAsync(string sku)
    {
        var result = await _service.CheckSkuAsync(sku);

        var dto = new CheckSkuResult
        {
            IsValid = result.IsValid,
            SuggestSku = result.SuggestSku,
            Reason = result.Reason
        };

        return Ok(dto);
    }

    [HttpGet("low-stock")]
    public async Task<QueryResult<LowStockModel>> GetLowStockProducts([FromQuery] QueryInfo queryInfo)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetLowStockProductsAsync(storeId, queryInfo);
    }
    [HttpPost("create")]
    [ValidateModel]
    public async Task<OperationResult> CreateProduct([FromForm] CreateProductModel model)
    {
        var result = await _service.CreateAsync(model);
        return result;
    }
    [HttpPost("bulk")]
    [AllowAnonymous]
    [ValidateModel]
    public async Task<OperationResult> CreateProducts([FromBody] List<CreateProductInfoModel> products)
    {

        var result = await _service.CreateAsync(products);
        return result;

        //var errors = new List<ValidationFailure>();

        //foreach (var product in products)
        //{
        //    var result = await _validator.ValidateAsync(product);
        //    if (!result.IsValid)
        //    {
        //        errors.AddRange(result.Errors);
        //    }
        //}

        //if (errors.Any())
        //{
        //    return new
        //    {
        //        title = "Validation failed",
        //        status = 400,
        //        detail = "One or more propety was validated.",
        //        errors = errors.Select(e => new { e.PropertyName, e.ErrorMessage })
        //    };
        //}
    }
    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> UpdateProduct([FromRoute] Guid id, [FromForm] ProductUpdateModel model)
    {
        var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
        var result = await _service.UpdateAsync(id, model, userId);

        if (result.Success)
        {
            var storeIds = await _storesService.GetStoresByProductIdAsync(id);
            foreach (var storeId in storeIds)
            {
                await _hubContext.Clients.Groups($"store-{storeId}")
                        .SendAsync("ProductDataChanged", new { action = "update", productId = id });
            }
        }
        return result;
    }

    [HttpPatch("delete/{id}")]
    public async Task<OperationResult> DeleteOrRestoreProduct([FromRoute] Guid id)
    {
        var result = await _service.DeleteOrRestoreProductAsync(id);
        if (result.Success)
        {
            var storeIds = await _storesService.GetStoresByProductIdAsync(id);
            foreach (var storeId in storeIds)
            {
                await _hubContext.Clients.Groups($"store-{storeId}")
                        .SendAsync("ProductDataChanged", new { action = "delete", productId = id });
            }
        }
        return result;
    }
}
