namespace Infrastructure;

public interface IPeriodLocksRepository : IRepositoryBase<PeriodLockEntity>
{
    Task<IEnumerable<InventoryPeriodLockEntity>> GetInventoryPeriodLockOfStoreAsync(Guid storeId);
}
