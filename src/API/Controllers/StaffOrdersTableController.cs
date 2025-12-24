using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/staff-orders")]
[ApiController]
[Authorize]
public class StaffOrdersTableController : ControllerBase
{
    private readonly IStaffOrdersTableService _staffOrderService;

    public StaffOrdersTableController(IStaffOrdersTableService staffOrderService)
    {
        _staffOrderService = staffOrderService;
    }

    private Guid StaffId
    {
        get
        {
            var idClaim = User.FindFirst("Id")
                       ?? User.FindFirst("id")
                       ?? User.FindFirst("sub")
                       ?? User.FindFirst(ClaimTypes.NameIdentifier);

            if (idClaim == null || string.IsNullOrWhiteSpace(idClaim.Value))
                throw new UnauthorizedAccessException("Không tìm thấy ID nhân viên trong token");

            if (!Guid.TryParse(idClaim.Value, out var staffId))
                throw new UnauthorizedAccessException("ID nhân viên không đúng định dạng Guid");

            return staffId;
        }
    }

    private Guid StoreId => HttpContextHelper.GetCurrentStoreId(HttpContext);

    // POST api/staff-orders/items → Thêm/cập nhật món vào bàn
    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddItemRequest request)
    {
        if (request.TableId == Guid.Empty)
            return BadRequest(new { message = "Vui lòng chọn bàn" });

        await _staffOrderService.AddOrUpdateItemAsync(
            staffId: StaffId,
            tableId: request.TableId,
            productId: request.ProductId,
            quantity: request.Quantity,
            storeId: StoreId,
            note: request.Note);

        return Ok(new { message = $"Đã thêm vào bàn {request.TableId}" });
    }

    // PATCH api/staff-orders/items → Cập nhật (hoặc thêm mới) một món trong bàn
    [HttpPatch("items")]
    public async Task<IActionResult> UpdateItem([FromBody] UpdateItemRequest request)
    {
        if (request.TableId == Guid.Empty)
            return BadRequest(new { message = "Vui lòng chọn bàn" });

        if (request.ProductId == Guid.Empty)
            return BadRequest(new { message = "ProductId không hợp lệ" });

        //if (request.Quantity < 1)
        //    return BadRequest(new { message = "Số lượng phải lớn hơn hoặc bằng 1" });

        await _staffOrderService.AddOrUpdateItemAsync(
            staffId: StaffId,
            tableId: request.TableId,
            productId: request.ProductId,
            quantity: request.Quantity,
            storeId: StoreId,
            note: request.Note);

        return Ok(new
        {
            message = "Cập nhật món thành công",
            tableId = request.TableId,
            productId = request.ProductId,
            quantity = request.Quantity,
            note = request.Note ?? "(không có ghi chú)"
        });
    }

    // POST api/staff-orders/items/batch
    [HttpPost("items/batch")]
    public async Task<IActionResult> AddItemsBatch([FromBody] AddItemsRequest request)
    {
        if (request.TableId == Guid.Empty)
            return BadRequest(new { message = "Vui lòng chọn bàn" });

        if (request.Items == null || request.Items.Count == 0)
            return BadRequest(new { message = "Chưa chọn món nào" });

        await _staffOrderService.AddOrUpdateMultipleItemsAsync(
            staffId: StaffId,
            tableId: request.TableId,
            items: request.Items,
            storeId: StoreId);

        return Ok(new
        {
            message = $"Đã thêm {request.Items.Count} món vào bàn {request.TableId}",
            count = request.Items.Count
        });
    }

    // GET api/staff-orders/items?tableId=xxx → Lấy giỏ hàng của bàn
    [HttpGet("items")]
    public async Task<IActionResult> GetItems([FromQuery] Guid tableId)
    {
        if (tableId == Guid.Empty)
            return BadRequest(new { message = "Vui lòng cung cấp bàn" });

        var items = await _staffOrderService.GetItemsAsync(StoreId, tableId);
        return Ok(items);
    }

    // DELETE api/staff-orders/items/{productId}?tableId=xxx → Xóa 1 món
    [HttpDelete("items/{productId}")]
    public async Task<IActionResult> RemoveItem(Guid productId, [FromQuery] Guid tableId)
    {
        if (tableId == Guid.Empty)
            return BadRequest(new { message = "Vui lòng cung cấp bàn" });

        await _staffOrderService.RemoveItemAsync(StoreId, tableId, productId);
        return Ok(new { message = "Đã xóa món khỏi bàn" });
    }

    // DELETE api/staff-orders/items?tableId=xxx → Xóa toàn bộ giỏ của bàn
    [HttpDelete("items")]
    public async Task<IActionResult> Clear([FromQuery] Guid tableId)
    {
        if (tableId == Guid.Empty)
            return BadRequest(new { message = "Vui lòng cung cấp bàn" });

        await _staffOrderService.ClearAllAsync(StoreId, tableId);
        return Ok(new { message = $"Đã xóa toàn bộ giỏ bàn {tableId}" });
    }

    /// <summary>
    /// Giảm số lượng món trong bàn - NHẬP TRỰC TIẾP SỐ LƯỢNG CẦN GIẢM
    /// </summary>
    [HttpPost("items/reduce-quantity")]
    public async Task<IActionResult> ReduceQuantity([FromBody] ReduceItemRequest request)
    {
        if (request.TableId == Guid.Empty)
            return BadRequest(new { message = "Vui lòng chọn bàn" });

        var result = await _staffOrderService.ReduceQuantityAsync(
            storeId: StoreId,
            tableId: request.TableId,
            productIdString: request.ProductId,
            reduceBy: request.QuantityToReduce);

        return Ok(new
        {
            message = result.WasRemoved
                ? $"Đã xóa món khỏi bàn {request.TableId} (hết số lượng)"
                : $"Đã giảm {request.QuantityToReduce} → còn lại {result.RemainingQuantity}",
            remaining = result.RemainingQuantity,
            removed = result.WasRemoved
        });
    }
}
