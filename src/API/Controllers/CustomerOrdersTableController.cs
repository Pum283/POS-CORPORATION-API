
namespace API.Controllers;

[Route("customer-orders-table")]
[ApiController]
public class CustomerOrdersTableController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok("This is API endpoint, not MVC view");
    }
}