using System.Security.Claims;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BuggyController(IExcelExporter<OrderDto> _excelExporter) : ControllerBase
{
    [HttpGet("orders")]
    public IActionResult DownloadOrders()
    {
        var data = new List<OrderDto>
        {
            new OrderDto { Id = 1, CustomerName = "Nguyen Van A", Amount = 1000 },
            new OrderDto { Id = 2, CustomerName = "Tran Van B", Amount = 2000 },
            new OrderDto { Id = 3, CustomerName = "Le Thi C", Amount = 3000 },
        };

        var fileContent = _excelExporter.ExportToExcel(data, "Orders");

        return File(fileContent,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Orders.xlsx");
    }

    [HttpGet("orders-multi-sheet")]
    public IActionResult DownloadOrdersMultiSheet()
    {
        var orders1 = new List<OrderDto>
        {
        new OrderDto { Id = 1, CustomerName = "Nguyen Van A", Amount = 1000 },
        new OrderDto { Id = 2, CustomerName = "Tran Van B", Amount = 2000 }
    };

        var orders2 = new List<OrderDto>
    {
        new OrderDto { Id = 3, CustomerName = "Le Thi C", Amount = 3000 },
        new OrderDto { Id = 4, CustomerName = "Pham Van D", Amount = 4000 }
    };

        var fileContent = _excelExporter.ExportToExcel(new Dictionary<string, IEnumerable<OrderDto>>
    {
        { "Orders1", orders1 },
        { "Orders2", orders2 }
    });

        return File(fileContent,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Orders-MultiSheet.xlsx");
    }
    [HttpGet("export-generic")]
    public IActionResult ExportGeneric()
    {
        var data = new List<OrderDto>
        {
        new OrderDto { Id = 1, CustomerName = "Nguyen Van A", Amount = 1000 },
        new OrderDto { Id = 2, CustomerName = "Tran Van B", Amount = 2000 }
        };

        var file = _excelExporter.ExportInvoice(
            data,
            "Cửa hàng Tú Tẻn",
            "Nguyễn Văn Tú",
            "Hóa đơn bán lẻ",
            DateTime.Now
        );

        return File(file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Invoice.xlsx");
    }



    [HttpGet("badrequest")]
    public IActionResult GetBadRequest()
    {
        throw new BadRequestException("Not a good request");
    }
    [HttpGet("notfound")]
    public IActionResult GetNotFound()
    {
        throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, "User", 1));
    }
    [HttpGet("internalerror")]
    public IActionResult GetInternaleError()
    {
        throw new Exception("This is a test internal exception");
    }
    [ValidateModel]
    [HttpPost("validationerror")]
    public IActionResult GetValidationError([FromBody] CreateUserModel request)
    {
        return Ok();
    }

    [Authorize]
    [HttpGet("secret")]
    public IActionResult Secret()
    {
        var name = User.FindFirstValue(ClaimTypes.Name);
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Ok("Hello " + name + "with the id of " + id);
    }
}
public class OrderDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}