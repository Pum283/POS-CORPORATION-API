namespace Infrastructure;

[RegisterService(typeof(IInventoryPeriodLocksRepository))]
public class InventoryPeriodLocksRepository(EfApplicationDbContext context) 
    : RepositoryBase<InventoryPeriodLockEntity>(context), IInventoryPeriodLocksRepository
{
    private readonly EfApplicationDbContext _context = context;
}
