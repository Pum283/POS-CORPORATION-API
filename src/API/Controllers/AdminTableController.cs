namespace API.Controllers;

[Route("api/tables")]
[ApiController]
//[Authorize(Roles = "Admin")]
public class TableController : ControllerBase
{
    private readonly ITableService _tableService;

    public TableController(ITableService tableService)
    {
        _tableService = tableService;
    }

    // Tạo bàn mới
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TableCreateRequest request)
    {
        var result = await _tableService.CreateAsync(request);
        return Ok(result);
    }

    // Lấy danh sách bàn (có thể lọc theo StoreId)
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? storeId)
    {
        var result = await _tableService.GetAllAsync(storeId);
        return Ok(result);
    }

    // Lấy chi tiết bàn
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _tableService.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    // Sửa bàn
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TableUpdateRequest request)
    {
        if (id != request.Id) return BadRequest();
        var success = await _tableService.UpdateAsync(request);
        if (!success) return NotFound();
        return Ok();
    }

    // Xóa bàn
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _tableService.DeleteAsync(id);
        if (!success) return NotFound();
        return Ok();
    }
}
