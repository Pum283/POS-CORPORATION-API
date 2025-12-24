namespace API.Controllers;

[Route("customers")]
public class CustomersController(ICustomersService _service) : BaseApiController
{
    [HttpGet]
    public async Task<QueryResult<Customer>> GetAllCustomers([FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetAllWithPagingAsync(queryInfo);
    }
    [HttpGet("{id}")]
    public async Task<Customer> GetCustomerById([FromRoute]Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result;
    }

    [HttpGet("{phoneNumber}/phone")]
    public async Task<List<Customer>> GetByPhoneNumber([FromRoute] string phoneNumber)
    {
        var result = await _service.GetByPhoneNumberAsync(phoneNumber);
        return result;
    }
    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> CreateCustomer([FromBody] CreateOrUpdateCustomerModel model)
    {
        var result = await _service.CreateAsync(model);
        return result;
    }
    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> UpdateCustomer([FromRoute] Guid id, [FromBody] CreateOrUpdateCustomerModel model)
    {
        var result = await _service.UpdateAsync(id, model);
        return result;
    }
    [HttpPost("loyaltypoint")]
    [ValidateModel]
    public async Task<OperationResult> CreateOrUpdateLoyaltyPoint(CreateLoyaltyPointModel model)
    {
        var result = await _service.CreateOrUpdateLoyaltyPointAsync(model);
        return result;
    }
}
