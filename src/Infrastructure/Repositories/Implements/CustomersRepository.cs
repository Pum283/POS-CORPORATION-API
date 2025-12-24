namespace Infrastructure;

[RegisterService(typeof(ICustomersRepository))]
public class CustomersRepository(EfApplicationDbContext context) : RepositoryBase<CustomerEntity>(context), ICustomersRepository
{
    private readonly EfApplicationDbContext _context = context;

    public async Task<QueryResult<Customer>> GetAllWithPagingAsync(QueryInfo queryInfo)
    {
        var searchText = queryInfo.SearchText?.Trim() ?? "";

        var query = from c in _context.Customers.AsNoTracking()
                    join lp in _context.LoyaltyPoints.AsNoTracking()
                        on c.Id equals lp.CustomerId into loyaltyPoints
                    from loyaltyPoint in loyaltyPoints.DefaultIfEmpty()
                    where EF.Functions.Collate(c.Name, "Latin1_General_CI_AI").Contains(searchText)
                    orderby c.CreateDate descending
                    select new Customer
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Phone = c.Phone,
                        Points = loyaltyPoint != null ? loyaltyPoint.Points : 0
                    };

        var pagedEntities = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        int totalCount = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalCount = await query.CountAsync();
        }

        return new QueryResult<Customer>
        {
            Data = pagedEntities,
            TotalCount = totalCount
        };
    }

    public async Task<Customer> GetCustomerByIdAsync(Guid id)
    {
        var customer = await (from c in _context.Customers.AsNoTracking()
                              where c.Id == id
                              join lp in _context.LoyaltyPoints.AsNoTracking() on c.Id equals lp.CustomerId into loyaltyPoints
                              from loyaltyPoint in loyaltyPoints.DefaultIfEmpty()
                              select new Customer
                              {
                                  Id = c.Id,
                                  Name = c.Name,
                                  Phone = c.Phone,
                                  Points = loyaltyPoint != null ? loyaltyPoint.Points : 0
                              }).FirstOrDefaultAsync();
        return customer ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Customer), id));
    }
    public async Task<List<Customer>> GetByPhoneNumberAsync(string phoneNumber)
    {
        var customer = await (from c in _context.Customers.AsNoTracking()
                              where c.Phone!.EndsWith(phoneNumber)
                              join lp in _context.LoyaltyPoints.AsNoTracking() on c.Id equals lp.CustomerId into loyaltyPoints
                              from loyaltyPoint in loyaltyPoints.DefaultIfEmpty()
                              select new Customer
                              {
                                  Id = c.Id,
                                  Name = c.Name,
                                  Phone = c.Phone,
                                  Points = loyaltyPoint != null ? loyaltyPoint.Points : 0
                              }).ToListAsync();
        return customer ?? throw new NotFoundException($"Customer with phone number {phoneNumber} was not found");
    }
}