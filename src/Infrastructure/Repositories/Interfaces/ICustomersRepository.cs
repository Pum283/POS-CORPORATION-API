namespace Infrastructure;

public interface ICustomersRepository : IRepositoryBase<CustomerEntity>
{
    Task<QueryResult<Customer>> GetAllWithPagingAsync(QueryInfo queryInfo);
    Task<Customer> GetCustomerByIdAsync(Guid id);
    Task<List<Customer>> GetByPhoneNumberAsync(string phoneNumber);
}
