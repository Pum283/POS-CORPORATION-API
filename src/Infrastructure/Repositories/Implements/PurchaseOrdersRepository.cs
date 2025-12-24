namespace Infrastructure;

[RegisterService(typeof(IPurchaseOrdersRepository))]
public class PurchaseOrdersRepository(EfApplicationDbContext context) : RepositoryBase<PurchaseOrderEntity>(context), IPurchaseOrdersRepository
{
    private readonly EfApplicationDbContext _context = context;

    public async Task<QueryResult<PurchaseOrder>> GetPurchaseOrderWithPagingAsync(QueryInfo queryInfo)
    {
        var query = from purchaseOrder in _context.PurchaseOrders.AsNoTracking()
                    join store in _context.Stores.AsNoTracking() on purchaseOrder.StoreId equals store.Id
                    join purchaseOrderItem in _context.PurchaseOrderItems.AsNoTracking()
                    on purchaseOrder.Id equals purchaseOrderItem.PurchaseOrderId
                    //where purchaseOrder.Id == Guid.Parse(queryInfo.SearchText ?? "")
                    //|| EF.Functions.Collate(orderGroup.Select(item), "Latin1_General_CI_AI").Contains(queryInfo.SearchText ?? "")
                    orderby purchaseOrder.ModifiedDate descending
                    group new { purchaseOrder, store, purchaseOrderItem } by purchaseOrder into orderGroup
                    select new PurchaseOrder
                    {
                        Id = orderGroup.FirstOrDefault()!.purchaseOrder.Id,
                        Store = new Store
                        {
                            Id = orderGroup.FirstOrDefault()!.store.Id,
                            Name = orderGroup.FirstOrDefault()!.store.Name,
                        },
                        Status = orderGroup.FirstOrDefault()!.purchaseOrder.Status,
                        TotalAmount = orderGroup.FirstOrDefault()!.purchaseOrder.TotalAmount,
                        PurchaseOrderItem = orderGroup.Select(item => new PurchaseOrderItem
                        {
                            ProductId = item.purchaseOrderItem.Product!.Id,
                            Product = new Product
                            {
                                Id = item.purchaseOrderItem.Product!.Id,
                                Name = item.purchaseOrderItem.Product.Name,
                                SKU = item.purchaseOrderItem.Product.SKU,
                                Picture = item.purchaseOrderItem.Product.Picture,
                                Price = item.purchaseOrderItem.Product.Price,
                            },
                            Quantity = item.purchaseOrderItem.Quantity
                        }

                        ).ToList()
                    };

        var pagedEntities = await query
            .OrderBy(x => queryInfo.OrderBy)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        int totalCount = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalCount = await query.CountAsync();
        }
        return new QueryResult<PurchaseOrder>
        {
            Data = pagedEntities,
            TotalCount = totalCount
        };
    }

    public async Task<PurchaseOrderEntity> GetPurchaseOrderWithDetailsByIdAsync(Guid id)
    {
        return await _context.PurchaseOrders
            .Include(x => x.PurchaseOrderItem)
            .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(PurchaseOrder), id));
    }
}
