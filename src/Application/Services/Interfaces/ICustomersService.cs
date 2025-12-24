namespace Application;

public interface ICustomersService
{
    Task<QueryResult<Customer>> GetAllWithPagingAsync(QueryInfo queryInfo);
    Task<Customer> GetByIdAsync(Guid id);
    Task<List<Customer>> GetByPhoneNumberAsync(string phoneNumber);
    Task<OperationResult> CreateAsync(CreateOrUpdateCustomerModel model);
    Task<OperationResult> CreateOrUpdateLoyaltyPointAsync(CreateLoyaltyPointModel model);
    Task<OperationResult> UpdateAsync(Guid id, CreateOrUpdateCustomerModel model);
}
