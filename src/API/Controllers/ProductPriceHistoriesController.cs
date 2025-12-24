namespace API.Controllers;

[Route("product-price-histories")]
[Authorize]
public class ProductPriceHistoriesController(
    IProductPriceHistoryService _service) : BaseApiController
{
    //    // Tất cả lịch sử
    //    [HttpGet]
    //    public async Task<QueryResult<ProductPriceHistoryModel>> GetAll([FromQuery] QueryInfo queryInfo)
    //        => await _service.GetAllAsync(queryInfo);

    //    // Chi tiết 1 bản ghi
    //    [HttpGet("{id:guid}")]
    //    public async Task<ActionResult<ProductPriceHistoryModel>> GetById([FromRoute] Guid id)
    //    {
    //        var item = await _service.GetByIdAsync(id);
    //        return item is null ? NotFound() : Ok(item);
    //    }

    //    // Theo sản phẩm
    //    [HttpGet("product/{productId:guid}")]
    //    public async Task<QueryResult<ProductPriceHistoryModel>> GetByProductId(
    //        [FromRoute] Guid productId, [FromQuery] QueryInfo queryInfo)
    //        => await _service.GetByProductIdAsync(productId, queryInfo);

    //    // Theo người dùng
    //    [HttpGet("user/{userId:guid}")]
    //    public async Task<QueryResult<ProductPriceHistoryModel>> GetByUserId(
    //        [FromRoute] Guid userId, [FromQuery] QueryInfo queryInfo)
    //        => await _service.GetByUserIdAsync(userId, queryInfo);

    //    // Theo cửa hàng (lấy từ StoreId của user thực hiện thay đổi)
    //    [HttpGet("store/{storeId:guid}")]
    //    public async Task<QueryResult<ProductPriceHistoryModel>> GetByStoreId(
    //        [FromRoute] Guid storeId,
    //        [FromQuery] QueryInfo queryInfo)
    //        => await _service.GetByStoreIdAsync(storeId, queryInfo);

    [HttpGet]
    public async Task<List<ProductPriceHistoryModel>> GetAll([FromQuery] ProductPriceHistoryDateFilter? filter = null)
    => await _service.GetAllAsync(filter);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductPriceHistoryModel>> GetById([FromRoute] Guid id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<List<ProductPriceHistoryModel>> GetByProductId(
        [FromRoute] Guid productId,
        [FromQuery] ProductPriceHistoryDateFilter? filter = null)
        => await _service.GetByProductIdAsync(productId, filter);

    [HttpGet("user/{userId:guid}")]
    public async Task<List<ProductPriceHistoryModel>> GetByUserId(
        [FromRoute] Guid userId,
        [FromQuery] ProductPriceHistoryDateFilter? filter = null)
        => await _service.GetByUserIdAsync(userId, filter);

    [HttpGet("store/{storeId:guid}")]
    public async Task<List<ProductPriceHistoryModel>> GetByStoreId(
        [FromRoute] Guid storeId,
        [FromQuery] ProductPriceHistoryDateFilter? filter = null)
        => await _service.GetByStoreIdAsync(storeId, filter);

}