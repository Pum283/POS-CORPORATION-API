namespace Infrastructure;

public class BankRepository(EfApplicationDbContext context) : RepositoryBase<BankEntity>(context), IBankRepository
{
    private readonly EfApplicationDbContext _context = context;
}
