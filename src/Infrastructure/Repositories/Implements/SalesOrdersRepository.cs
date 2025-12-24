namespace Infrastructure;

[RegisterService(typeof(ISalesOrdersRepository))]
public class SalesOrdersRepository(
    EfApplicationDbContext context,
    IMapper _mapper) : RepositoryBase<SalesOrderEntity>(context), ISalesOrdersRepository
{
    private readonly EfApplicationDbContext _context = context;

    public async Task<QueryResult<SalesOrderModel>> GetSalesOrdersWithPagingAsync(Guid storeId, QueryInfo queryInfo, SalesOrderQuery orderQuery)
    {
        var fromDate = orderQuery.DateTime;
        var toDate = orderQuery.DateTime?.AddDays(1);

        var query = _context.SalesOrders.AsNoTracking()
            .Where(x => x.StoreId == storeId
                        && x.OrderStatus == OrderStatusType.Confirmed
                        && x.ShippingType == Enum.Parse<ShippingType>(orderQuery.ShippingType!)
                        && (x.ModifiedDate >= fromDate && x.ModifiedDate < toDate))
            .OrderByDescending(x => x.CreateDate)
            .Select(order => new SalesOrderModel
            {
                Id = order.Id,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod.ToString(),
                CreateDate = order.CreateDate.ToString("HH:mm"),
            });

        if (!string.IsNullOrEmpty(queryInfo.SearchText))
        {
            query = query.Where(x => x.Id.ToString() == queryInfo.SearchText);
        }

        var totalCount = queryInfo.NeedTotalCount
            ? await query.CountAsync()
            : 0;

        var pagedEntities = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        return new QueryResult<SalesOrderModel>
        {
            Data = pagedEntities,
            TotalCount = totalCount,
        };
    }



    public async Task<SalesOrder> GetSalesOrderByIdForUpdateSOAsync(Guid id)
    {
        var order = await _context.SalesOrders
            .Where(x => x.Id == id && x.OrderStatus == OrderStatusType.Pending)
            .Select(order => new SalesOrder
            {
                Id = order.Id,
                StoreId = order.StoreId
            })
            .FirstOrDefaultAsync();

        if (order == null) throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(SalesOrder), id));

        var orderItems = await _context.SalesOrderItems
            .AsNoTracking()
            .Where(x => x.SalesOrderId == id)
            .Select(item => new SalesOrderItem
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
            })
            .ToListAsync();

        order.SalesOrderItem = orderItems;
        return _mapper.Map<SalesOrder>(order);
    }

    public async Task<SalesOrderModel> GetSalesOrderByIdAsync(Guid id)
    {
        var order = await _context.SalesOrders
            .Where(x => x.Id == id && x.OrderStatus == OrderStatusType.Confirmed)
            //.Join(_context.Customers,
            //    order => order.CustomerId,
            //    customer => customer.Id,
            //    (order, customer) => new { order, customer })
            .Select(order => new SalesOrderModel
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                CustomerName = order.Customer != null ? order.Customer.Name : "",
                UserName = order.User != null ? order.User.Name : "",
                TotalAmount = order.TotalAmount,
                Discount = order.Discount,
                PaymentMethod = order.PaymentMethod.ToString(),
                PaidAmount = order.PaidAmount,
                CreateDate = order.CreateDate.ToString("dd-MM-yyyy"),
                SalesOrderItems = order.SalesOrderItem != null
                ? order.SalesOrderItem.Select(item => new SalesOrderItemModel
                {
                    Id = item.Id,
                    ProductName = item.Product != null ? item.Product.Name : null,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                }).ToList()
                : new List<SalesOrderItemModel>()
            }).FirstOrDefaultAsync();

        return order ?? new SalesOrderModel();
    }

    public async Task<QueryResult<SalesOrderOfCustomerModel>> GetSalesOrdersOfCustomerWithPagingAsync(Guid customerId, QueryInfo queryInfo)
    {
        var query = _context.SalesOrders.AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreateDate)
            .Select(order => new SalesOrderOfCustomerModel
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer != null ? order.Customer.Name : string.Empty,
                StoreName = order.Store != null ? order.Store.Name : string.Empty,
                OrderCode = order.OrderCode,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod.ToString(),
                ShippingType = order.ShippingType.ToString(),
                Time = order.CreateDate.ToString("dd-MM-yyyy hh:mm tt")
            });

        var totalCount = queryInfo.NeedTotalCount
            ? await query.CountAsync()
            : 0;

        var pagedEntities = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        return new QueryResult<SalesOrderOfCustomerModel>
        {
            Data = pagedEntities,
            TotalCount = totalCount,
        };
    }
}
