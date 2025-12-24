namespace Infrastructure;

[RegisterService(typeof(IPeriodLocksRepository))]
public class PeriodLocksRepository(EfApplicationDbContext context) : RepositoryBase<PeriodLockEntity>(context), IPeriodLocksRepository
{
    private readonly EfApplicationDbContext _context = context;

    public async Task<IEnumerable<InventoryPeriodLockEntity>> GetInventoryPeriodLockOfStoreAsync(Guid storeId)
    {
        /** Cách 1: Dùng LINQ
         * Cách này sẽ load hết data về memory rồi mới xử lí, nếu data nhiều sẽ rất chậm
         */
        //var entities = await (
        //                from lockItem in _context.InventoryPeriodLocks
        //                where lockItem.StoreId == storeId
        //                join latest in (
        //                    from l in _context.InventoryPeriodLocks
        //                    where l.StoreId == storeId
        //                    group l by new { l.ProductId, l.IngredientId } into g
        //                    select new
        //                    {
        //                        g.Key.ProductId,
        //                        g.Key.IngredientId,
        //                        MaxDate = g.Max(x => x.CreateDate)
        //                    }
        //                ) on new { lockItem.ProductId, lockItem.IngredientId, lockItem.CreateDate }
        //                   equals new { latest.ProductId, latest.IngredientId, CreateDate = latest.MaxDate }
        //                select new InventoryPeriodLockEntity
        //                {
        //                    Id = lockItem.Id,
        //                    StoreId = lockItem.StoreId,
        //                    ProductId = lockItem.ProductId,
        //                    IngredientId = lockItem.IngredientId,
        //                    OpeningQuantity = lockItem.OpeningQuantity,
        //                    ClosingQuantity = lockItem.ClosingQuantity,
        //                    EditStatus = lockItem.EditStatus,
        //                    CreateDate = lockItem.CreateDate,
        //                    ModifiedDate = lockItem.ModifiedDate
        //                }).ToListAsync();

        /** Cách 2: Dùng Raw SQL
         * Cách này sẽ xử lí ở sql server, nhanh hơn rất nhiều
         * Lưu ý: Cách này sẽ không kiểm tra được SQL Injection, nên chỉ dùng với các biến đã được kiểm soát chặt chẽ
         * Ví dụ như biến storeId này là Guid do hệ thống tạo ra, không phải do người dùng nhập vào thì không sợ SQL Injection
         */

        var entities = await _context.InventoryPeriodLocks
                        .FromSqlRaw(@"
                            SELECT *
                            FROM (
                                SELECT *,
                                       ROW_NUMBER() OVER (
                                           PARTITION BY ProductId, IngredientId
                                           ORDER BY CreateDate DESC
                                       ) AS rn
                                FROM InventoryPeriodLocks
                                WHERE StoreId = {0}
                            ) t
                            WHERE rn = 1
                        ", storeId)
                        .Select(lockItem => new InventoryPeriodLockEntity
                        {
                            Id = lockItem.Id,
                            StoreId = lockItem.StoreId,
                            ProductId = lockItem.ProductId,
                            IngredientId = lockItem.IngredientId,
                            OpeningQuantity = lockItem.OpeningQuantity,
                            ClosingQuantity = lockItem.ClosingQuantity,
                            EditStatus = lockItem.EditStatus,
                            CreateDate = lockItem.CreateDate,
                            ModifiedDate = lockItem.ModifiedDate
                        })
                        .ToListAsync();

        return entities;
    }
}
