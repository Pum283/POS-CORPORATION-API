using System.Security.Cryptography.X509Certificates;
using Domain.Models;

namespace Infrastructure;

[RegisterService(typeof(IDashboardRepository))]
public class DashboardRepository(EfApplicationDbContext context) : IDashboardRepository
{
    private readonly EfApplicationDbContext _context = context;

    //public async Task<RevenueReportModel> GetRevenueReportAsync(QueryInfo queryInfo, GetRevenueReportQuery query)
    //{
    //    var fromTime = query.FromDate;
    //    var toTime = query.ToDate;
    //    var orderQuery = _context.SalesOrders.AsNoTracking()
    //                    .Join(_context.SalesOrderItems.AsNoTracking(),
    //                        order => order.Id,
    //                        item => item.SalesOrderId,
    //                        (order, item) => new { order, item })
    //                    .Where(o => o.order.OrderStatus == OrderStatusType.Confirmed);

    //    if (query.StoreId.HasValue)
    //    {
    //        orderQuery = orderQuery.Where(o => o.order.StoreId == query.StoreId.Value);
    //    }
    //    if (fromTime.HasValue)
    //    {
    //        orderQuery = orderQuery.Where(x => x.order.ModifiedDate >= fromTime.Value);
    //    }

    //    if (toTime.HasValue)
    //    {
    //        orderQuery = orderQuery.Where(x => x.order.ModifiedDate < toTime.Value);
    //    }

    //    if (query.ProductGroupId.HasValue)
    //    {
    //        orderQuery = orderQuery.Where(o => o.item.Product!.ProductGroupId == query.ProductGroupId.Value);
    //    }
    //    if (query.ProductId.HasValue)
    //    {
    //        orderQuery = orderQuery.Where(o => o.item.ProductId == query.ProductId.Value);
    //    }

    //    var storeOrders = from order in orderQuery
    //                      join store in _context.Stores on order.order.StoreId equals store.Id
    //                      select new
    //                      {
    //                          store.Id,
    //                          store.Name,
    //                          order.order.PaidAmount
    //                      };

    //    var grouped = await storeOrders
    //                    .GroupBy(x => new { x.Id, x.Name })
    //                    .Select(g => new RevenueStoreDetailModel
    //                    {
    //                        StoreName = g.Key.Name,
    //                        Orders = g.Count(),
    //                        Revenue = g.Sum(x => x.PaidAmount)
    //                    })
    //                    .ToListAsync();

    //    var totalRevenue = grouped.Sum(x => x.Revenue);
    //    var totalOrders = grouped.Sum(x => x.Orders);
    //    var totalStores = grouped.Count;

    //    var summary = new RevenueReportSummaryModel
    //    {
    //        TotalRevenue = totalRevenue,
    //        TotalOrders = totalOrders,
    //        TotalStores = totalStores,
    //        AverageRevenuePerStore = totalStores > 0 ? totalRevenue / totalStores : 0
    //    };

    //    var pagedEntities = grouped
    //        .Skip(queryInfo.Skip)
    //        .Take(queryInfo.Top)
    //        .ToList();

    //    var stores = new QueryResult<RevenueStoreDetailModel>
    //    {
    //        Data = pagedEntities,
    //        TotalCount = grouped.Count
    //    };

    //    return new RevenueReportModel
    //    {
    //        Summary = summary,
    //        Stores = stores
    //    };
    //}

    public async Task<RevenueReportModel> GetRevenueReportAsync(QueryInfo queryInfo, GetRevenueReportQuery query)
    {
        // Lấy khoảng thời gian từ query
        var fromTime = query.FromDate;
        var toTime = query.ToDate;

        // Bắt đầu từ bảng SalesOrders (mỗi order chỉ 1 record ở đây)
        var orders = _context.SalesOrders
                             .AsNoTracking()
                             .Where(o => o.OrderStatus == OrderStatusType.Confirmed);

        // Lọc theo Store nếu có
        if (query.StoreId.HasValue)
        {
            orders = orders.Where(o => o.StoreId == query.StoreId.Value);
        }

        // Lọc theo fromTime nếu có
        if (fromTime.HasValue)
        {
            orders = orders.Where(o => o.ModifiedDate >= fromTime.Value);
        }

        // Lọc theo toTime nếu có
        if (toTime.HasValue)
        {
            orders = orders.Where(o => o.ModifiedDate < toTime.Value);
        }

        // Nếu cần lọc theo ProductGroup hoặc Product riêng lẻ,
        // lấy danh sách orderId từ SalesOrderItems phù hợp, rồi apply vào orders.
        if (query.ProductGroupId.HasValue || query.ProductId.HasValue)
        {
            var itemsQuery = _context.SalesOrderItems.AsNoTracking().AsQueryable();

            if (query.ProductGroupId.HasValue)
            {
                itemsQuery = itemsQuery.Where(i => i.Product!.ProductGroupId == query.ProductGroupId.Value);
            }

            if (query.ProductId.HasValue)
            {
                itemsQuery = itemsQuery.Where(i => i.ProductId == query.ProductId.Value);
            }

            // Lấy các SalesOrderId thỏa điều kiện (distinct để tránh duplicate)
            var orderIds = itemsQuery.Select(i => i.SalesOrderId).Distinct();

            // Giữ các orders có Id nằm trong orderIds
            orders = orders.Where(o => orderIds.Contains(o.Id));
        }

        // Join với Stores để lấy tên cửa hàng và PaidAmount từ order (mỗi order chỉ xuất hiện 1 lần)
        var storeOrders = from order in orders
                          join store in _context.Stores.AsNoTracking() on order.StoreId equals store.Id
                          select new
                          {
                              store.Id,
                              store.Name,
                              order.PaidAmount
                          };

        // Nhóm theo cửa hàng, đếm số order (mỗi order 1 lần) và tổng PaidAmount (không bị nhân theo số item)
        var grouped = await storeOrders
                        .GroupBy(x => new { x.Id, x.Name })
                        .Select(g => new RevenueStoreDetailModel
                        {
                            StoreName = g.Key.Name,
                            Orders = g.Count(),
                            Revenue = g.Sum(x => x.PaidAmount)
                        })
                        .ToListAsync();

        // Tính tổng summary
        var totalRevenue = grouped.Sum(x => x.Revenue);
        var totalOrders = grouped.Sum(x => x.Orders);
        var totalStores = grouped.Count;

        var summary = new RevenueReportSummaryModel
        {
            TotalRevenue = totalRevenue,
            TotalOrders = totalOrders,
            TotalStores = totalStores,
            AverageRevenuePerStore = totalStores > 0 ? totalRevenue / totalStores : 0
        };

        // Phân trang trên danh sách đã nhóm
        var pagedEntities = grouped
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToList();

        var stores = new QueryResult<RevenueStoreDetailModel>
        {
            Data = pagedEntities,
            TotalCount = grouped.Count
        };

        return new RevenueReportModel
        {
            Summary = summary,
            Stores = stores
        };
    }


    public async Task<DailyClosingReportModel> GetDailyReportAsync(/*QueryInfo queryInfo, */GetDailyClosingReportQuery query)
    {
        var date = query.Date.Date;
        var nextDate = date.AddDays(1);

        var orderQuery = _context.SalesOrders
                .AsNoTracking()
                .Where(o => o.OrderStatus == OrderStatusType.Confirmed &&
                o.StoreId == query.StoreId &&
                o.ModifiedDate >= date &&
                o.ModifiedDate < nextDate);

        var rawProductOrders = await (
            from order in orderQuery
            join item in _context.SalesOrderItems.AsNoTracking() on order.Id equals item.SalesOrderId
            join products in _context.Products.AsNoTracking() on item.ProductId equals products.Id
            join productGroup in _context.ProductGroups.AsNoTracking() on products.ProductGroupId equals productGroup.Id
            select new
            {
                OrderId = order.Id,
                order.PaidAmount,
                order.PaymentMethod,
                ProductName = products.Name,
                ProductGroup = productGroup.Name,
                Quantity = item.Quantity,
                ItemTotalPrice = item.TotalPrice
            }
        ).ToListAsync();

        var orderItemTotals = rawProductOrders
            .GroupBy(x => x.OrderId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.ItemTotalPrice));

        var productOrders = rawProductOrders
            .Where(x => orderItemTotals[x.OrderId] > 0)
            .Select(x =>
            {
                var total = orderItemTotals[x.OrderId];
                var ratio = x.ItemTotalPrice / total;
                var itemRevenue = x.PaidAmount * ratio;
                var roundedRevenue = Math.Round(itemRevenue, 0);

                return new DailyProductDetailModel
                {
                    ProductName = x.ProductName,
                    ProductGroup = x.ProductGroup,
                    QuantitySold = x.Quantity,
                    Revenue = roundedRevenue,
                    PaymentMethods = new List<PaymentDetailModel>
                    {
                new PaymentDetailModel
                {
                    Method = x.PaymentMethod.ToString()!,
                    Amount = roundedRevenue
                }
                    }
                };
            })
            .ToList();


        var groupedProducts = productOrders
        .GroupBy(p => new { p.ProductName, p.ProductGroup })
        .Select(g => new DailyProductDetailModel
        {
            ProductName = g.Key.ProductName,
            ProductGroup = g.Key.ProductGroup,
            QuantitySold = g.Sum(x => x.QuantitySold),
            Revenue = g.Sum(x => x.Revenue),
            PaymentMethods = g
            .SelectMany(x => x.PaymentMethods)
            .GroupBy(pm => pm.Method)
            .Select(gp => new PaymentDetailModel
            {
                Method = gp.Key,
                Amount = gp.Sum(x => x.Amount)
            })
            .ToList()
        })
        .ToList();

        var totalRevenue = groupedProducts.Sum(x => x.Revenue);
        var totalQuantity = groupedProducts.Sum(x => x.QuantitySold);
        var totalTransactions = await orderQuery.CountAsync();

        var totalMoneyReturn = await CaculateTotalMoneyReturn(date, nextDate, query.StoreId);

        var summary = new DailyClosingSummaryModel
        {
            TotalRevenue = Math.Round(totalRevenue - totalMoneyReturn.TotalMoney, 0),
            TotalQuantity = totalQuantity - totalMoneyReturn.TotalQuantity,
            TotalTransactions = totalTransactions,
            ReportDate = query.Date.Date
        };

        var pagedEntities = groupedProducts
        //.Skip(queryInfo.Skip)
        //.Take(queryInfo.Top)
        .ToList();

        var productResult = new QueryResult<DailyProductDetailModel>
        {
            Data = pagedEntities,
            TotalCount = groupedProducts.Count
        };

        return new DailyClosingReportModel
        {
            Summary = summary,
            Products = productResult
        };
    }

    public async Task<MainDiagramReportModel> GetMainDiagramReportAsync(QueryInfo queryInfo, MainDiagramReportQuery query)
    {
        ShippingType parsedShippingType = default;
        var hasShippingType = !string.IsNullOrEmpty(query.ShippingType) &&
                              Enum.TryParse(query.ShippingType, out parsedShippingType);

        var orderQuery = _context.SalesOrders.AsNoTracking()
            .Where(o => o.StoreId == query.StoreId &&
                        o.OrderStatus == OrderStatusType.Confirmed &&
                        (!hasShippingType || o.ShippingType == parsedShippingType) &&
                        o.ModifiedDate >= query.Date &&
                        o.ModifiedDate < query.Date.AddDays(1));

        var groupedProducts = await orderQuery.GroupBy(o => o.Id)
            .Select(g => new MainDiagramOrderDetailModel
            {
                OrderName = g.Key.ToString(),
                Type = g.FirstOrDefault()!.PaymentMethod.ToString()!,
                Time = g.Min(x => x.ModifiedDate).ToString("HH:mm"),
                Amount = g.Sum(x => x.PaidAmount),
            }
           )
            .ToListAsync();

        var totalPriceOfDocumentByReturnedGoodsReceiptType = _context.DocumentSuppliers.AsNoTracking()
            .Include(x => x.Product)
            .Where(x => x.TransType == TransType.ReturnOutput)
            .Where(x => x.StoreId == query.StoreId &&
                        x.ModifiedDate >= query.Date &&
                        x.ModifiedDate < query.Date.AddDays(1))
            .Sum(x => x.Quantity * (x.Product != null ? x.Product.Price : 0));

        var totalOrders = groupedProducts.Count();
        var totalRevenue = groupedProducts.Sum(x => x.Amount);

        var summaryResult = new MainDiagramReportSummaryModel
        {
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            Date = query.Date.ToString("yyyy-MM-dd"),
        };

        var pagedEntities = groupedProducts
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToList();

        var productsResult = new QueryResult<MainDiagramOrderDetailModel>
        {
            Data = pagedEntities,
            TotalCount = groupedProducts.Count()
        };
        return new MainDiagramReportModel
        {
            Summary = summaryResult,
            Orders = productsResult
        };
    }

    public async Task<InventoryReportModel> GetInventoryReportAsync(Guid storeId, QueryInfo queryInfo, InventoryDetailReportItemModel query)
    {
        var todayStart = DateTime.UtcNow.Date;
        var tomorrowStart = todayStart.AddDays(1);
        var fromDate7Days = todayStart.AddDays(-6);

        // 1. Truy vấn dữ liệu tồn kho và sản phẩm
        var inventories = await (
            from i in _context.Inventories.AsNoTracking()
            join p in _context.Products.AsNoTracking() on i.ProductId equals p.Id
            where i.StoreId == storeId
            select new
            {
                Inventory = i,
                Product = p
            }
        ).ToListAsync();

        var inventoryDict = inventories.ToDictionary(x => x.Inventory.ProductId!);

        // 2. Truy vấn chi tiết bán hàng cả 7 ngày và hôm nay
        var allSoldDetails = await (
            from so in _context.SalesOrders.AsNoTracking()
            join soi in _context.SalesOrderItems.AsNoTracking() on so.Id equals soi.SalesOrderId
            where so.StoreId == storeId
                  && so.OrderStatus == OrderStatusType.Confirmed
                  && so.ModifiedDate >= fromDate7Days
                  && so.ModifiedDate < tomorrowStart
            select new
            {
                soi.ProductId,
                so.ShippingType,
                so.ModifiedDate,
                soi.Quantity
            }
        ).ToListAsync();

        // 3. Gom nhóm
        var soldDetails7DaysGrouped = allSoldDetails
            .GroupBy(x => new { x.ProductId, x.ShippingType })
            .Select(g => new
            {
                g.Key.ProductId,
                g.Key.ShippingType,
                Quantity = g.Sum(x => x.Quantity),
                Dates = g.Select(x => x.ModifiedDate)
            })
            .ToList();

        var soldDetailsTodayGrouped = soldDetails7DaysGrouped
            .Where(x => x.Dates.Any(d => d >= todayStart && d < tomorrowStart))
            .Select(x => new
            {
                x.ProductId,
                x.ShippingType,
                Quantity = allSoldDetails
                    .Where(sd => sd.ProductId == x.ProductId && sd.ShippingType == x.ShippingType &&
                                 sd.ModifiedDate >= todayStart && sd.ModifiedDate < tomorrowStart)
                    .Sum(sd => sd.Quantity)
            }).ToList();

        // 4. Lấy original quantity 1 lần
        var productIds = inventoryDict.Keys.ToList();
        var originalQuantities = await _context.InventoryPeriodLocks
            .Where(x => productIds.Contains(x.ProductId!.Value) && x.StoreId == storeId)
            .GroupBy(x => x.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                OpeningQuantity = g.Max(x => x.OpeningQuantity)
            })
            .ToDictionaryAsync(x => x.ProductId!.Value, x => x.OpeningQuantity);

        // 5. Tổng hợp cảnh báo
        //var warnings = new List<InventoryWarningModel>();
        var productSummaries = new List<ProductInventorySummaryModel>();

        foreach (var kv in inventoryDict)
        {
            var productId = kv.Key;
            var inventory = kv.Value.Inventory;
            var product = kv.Value.Product;

            // Sold 7 ngày
            var sold7Days = soldDetails7DaysGrouped
                .Where(x => x.ProductId == productId)
                .ToDictionary(x => x.ShippingType, x => x.Quantity);

            var totalSold7Days = sold7Days.Values.Sum();
            var avgPerDay = totalSold7Days / 7.0;


            // Sold hôm nay
            var soldToday = soldDetailsTodayGrouped
                .Where(x => x.ProductId == productId)
                .ToDictionary(x => x.ShippingType, x => x.Quantity);

            var totalSoldToday = soldToday.Values.Sum();
            var originalQuantity = originalQuantities.ContainsKey(productId) ? originalQuantities[productId] : 0;
            var inputQuantity = _context.DocumentSuppliers
                .Where(x => x.StoreId == storeId && x.ProductId == productId &&
                            x.TransType == TransType.GoodsReceipt &&
                            (x.ModifiedDate >= todayStart && x.ModifiedDate < tomorrowStart))
                .Sum(x => x.Quantity);

            var cancelProductedQuantity = _context.DocumentSuppliers
                .Where(x => x.StoreId == storeId && x.ProductId == productId &&
                            x.TransType == TransType.CanceledProduct &&
                            (x.ModifiedDate >= todayStart && x.ModifiedDate < tomorrowStart))
                .Sum(x => x.Quantity);
            productSummaries.Add(new ProductInventorySummaryModel
            {
                SKU = product.SKU,
                Name = product.Name,
                Unit = product.Unit.ToString(),
                OriginalQuantity = originalQuantity,
                BuyQuantity = totalSoldToday,
                Quantity = inputQuantity - totalSoldToday - cancelProductedQuantity,
                InputQuantity = inputQuantity,
                SoldByShippingType = soldToday
            });
        }


        // 7. Phân trang
        var productSummariesPaged = productSummaries
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToList();

        var productSummariesResult = new QueryResult<ProductInventorySummaryModel>
        {
            Data = productSummariesPaged,
            TotalCount = productSummaries.Count
        };

        var summary = new InventoryReportSummaryModel
        {
            TotalProduct = inventoryDict.Count(),
            //TotalLowStockWarnings = warnings,
            //TopProducts = topSelling
        };

        return new InventoryReportModel
        {
            //Summary = summary,
            Products = productSummariesResult
        };
    }
    public async Task<InventoryReportSummaryModel> GetInventorySummaryReportAsync(Guid storeId, DateTime? date = null)
    {
        var todayStart = (date ?? DateTime.UtcNow).Date;
        var tomorrowStart = todayStart.AddDays(1);
        var fromDate7Days = todayStart.AddDays(-6);

        var inventories = await (
            from i in _context.Inventories.AsNoTracking()
            join p in _context.Products.AsNoTracking() on i.ProductId equals p.Id
            where i.StoreId == storeId && p.IsActive
            select new { Inventory = i, Product = p }
        ).ToListAsync();

        var inventoryDict = inventories.ToDictionary(x => x.Inventory.ProductId);

        var allSoldDetails = await (
            from so in _context.SalesOrders.AsNoTracking()
            join soi in _context.SalesOrderItems.AsNoTracking() on so.Id equals soi.SalesOrderId
            where so.StoreId == storeId
                  && so.OrderStatus == OrderStatusType.Confirmed
                  && so.ModifiedDate >= fromDate7Days && so.ModifiedDate < tomorrowStart
            select new
            {
                soi.ProductId,
                so.ShippingType,
                so.ModifiedDate,
                soi.Quantity
            }
        ).ToListAsync();

        var soldGrouped = allSoldDetails
            .GroupBy(x => new { x.ProductId, x.ShippingType })
            .Select(g => new
            {
                g.Key.ProductId,
                g.Key.ShippingType,
                Quantity = g.Sum(x => x.Quantity),
                Dates = g.Select(x => x.ModifiedDate)
            })
            .ToList();

        var warnings = new List<InventoryWarningModel>();

        foreach (var item in inventoryDict)
        {
            var productId = item.Key;
            var inventory = item.Value.Inventory;
            var product = item.Value.Product;

            var sold7Days = soldGrouped
                .Where(x => x.ProductId == productId)
                .Sum(x => x.Quantity);

            var avgPerDay = sold7Days / 7.0;

            if (inventory.Quantity < (decimal)avgPerDay)
            {
                warnings.Add(new InventoryWarningModel
                {
                    SKU = product.SKU,
                    Name = product.Name,
                    InventoryQuantity = inventory.Quantity
                });
            }
            if (inventory.Quantity < 3)
            {
                warnings.Add(new InventoryWarningModel
                {
                    SKU = product.SKU,
                    Name = product.Name,
                    InventoryQuantity = inventory.Quantity
                });
            }
        }

        var topSelling = soldGrouped
            .GroupBy(x => x.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalSold = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.TotalSold)
            .Take(5)
            .Select(x =>
            {
                var prod = inventoryDict.ContainsKey(x.ProductId) ? inventoryDict[x.ProductId].Product : null;
                return new TopProductBestSellingModel
                {
                    SKU = prod?.SKU ?? "",
                    Name = prod?.Name ?? "",
                    SoldQuantity = x.TotalSold
                };
            }).ToList();

        return new InventoryReportSummaryModel
        {
            TotalProduct = inventoryDict.Count,
            TotalLowStockWarnings = warnings
                .Where(x => !string.IsNullOrEmpty(x.SKU))
                .GroupBy(x => x.SKU)
                .Select(g => g.First())
                .ToHashSet(),
            TopProducts = topSelling
        };
    }

    public async Task<QueryResult<ProductInventorySummaryModel>> GetInventoryProductSummariesAsync(
      Guid storeId, QueryInfo queryInfo, DateTime? date = null)
    {
        var todayStart = await _context.PeriodLocks
            .Where(x => x.StoreId == storeId && x.Status == ShiftPeriodStatusType.Started)
            .Select(x => x.CreateDate)
            .FirstOrDefaultAsync();
        var tomorrowStart = todayStart.AddHours(12);

        // 1. Query phân trang tồn kho + sản phẩm
        var inventoryQuery =
            from i in _context.Inventories.AsNoTracking()
            join p in _context.Products.AsNoTracking() on i.ProductId equals p.Id
            where i.StoreId == storeId && p.IsActive == queryInfo.IsActive
            select new { Inventory = i, Product = p };

        var totalCount = await inventoryQuery.CountAsync();

        var inventories = await inventoryQuery
            .OrderBy(x => x.Product.Name)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        var productIds = inventories.Select(x => x.Inventory.ProductId).ToList();

        // 2. Opening quantity (chú ý filter theo kỳ thay vì Max)
        var originalQuantities = await _context.InventoryPeriodLocks
            .Where(x => productIds.Contains(x.ProductId!.Value) && x.StoreId == storeId)
            .GroupBy(x => x.ProductId)
            .Select(g => new
            {
                ProductId = g.Key!.Value,
                OpeningQuantity = g.Max(x => x.OpeningQuantity) // FIXME: filter theo kỳ cho đúng
            })
            .ToDictionaryAsync(x => x.ProductId, x => x.OpeningQuantity);

        // 3. Sold today (Confirmed orders)
        var soldToday = await (
            from so in _context.SalesOrders.AsNoTracking()
            join soi in _context.SalesOrderItems.AsNoTracking() on so.Id equals soi.SalesOrderId
            where so.StoreId == storeId
                  && so.OrderStatus == OrderStatusType.Confirmed
                  && so.ModifiedDate >= todayStart && so.ModifiedDate < tomorrowStart
                  && productIds.Contains(soi.ProductId)
            group soi by soi.ProductId into g
            select new { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) }
        ).ToDictionaryAsync(x => x.ProductId, x => x.Quantity);

        // 3.1. Sold by ShippingType
        var soldByShippingType = await (
            from so in _context.SalesOrders.AsNoTracking()
            join soi in _context.SalesOrderItems.AsNoTracking() on so.Id equals soi.SalesOrderId
            where so.StoreId == storeId
                  && so.OrderStatus == OrderStatusType.Confirmed
                  && so.ModifiedDate >= todayStart && so.ModifiedDate < tomorrowStart
                  && productIds.Contains(soi.ProductId)
            group soi by new { soi.ProductId, so.ShippingType } into g
            select new
            {
                ProductId = g.Key.ProductId,
                ShippingType = g.Key.ShippingType,
                Quantity = g.Sum(x => x.Quantity)
            }
        ).ToListAsync();

        var soldByShippingTypeDict = soldByShippingType
            .GroupBy(x => x.ProductId)
            .ToDictionary(
                g => g.Key,
                g => g.GroupBy(x => x.ShippingType)
                      .ToDictionary(gg => gg.Key, gg => gg.Sum(x => x.Quantity))
            );

        // 4. Input quantity
        var inputQuantities = await _context.DocumentSuppliers
            .Where(x => x.StoreId == storeId
                        && x.TransType == TransType.GoodsReceipt
                        && x.ModifiedDate >= todayStart && x.ModifiedDate < tomorrowStart
                        && x.ProductId.HasValue
                        && productIds.Contains(x.ProductId.Value))
            .GroupBy(x => x.ProductId)
            .Select(g => new { ProductId = g.Key!.Value, Quantity = g.Sum(x => x.Quantity) })
            .ToDictionaryAsync(x => x.ProductId, x => x.Quantity);

        // 5. Canceled quantity
        var canceledQuantities = await _context.DocumentSuppliers
            .Where(x => x.StoreId == storeId
                        && x.TransType == TransType.CanceledProduct
                        && x.ModifiedDate >= todayStart && x.ModifiedDate < tomorrowStart
                        && x.ProductId.HasValue
                        && productIds.Contains(x.ProductId.Value))
            .GroupBy(x => x.ProductId)
            .Select(g => new { ProductId = g.Key!.Value, Quantity = g.Sum(x => x.Quantity) })
            .ToDictionaryAsync(x => x.ProductId, x => x.Quantity);

        // 6. Tổng hợp kết quả
        var summaries = inventories.Select(x =>
        {
            var productId = x.Inventory.ProductId;

            var original = originalQuantities.TryGetValue(productId, out var oq) ? oq : 0;
            var sold = soldToday.TryGetValue(productId, out var s) ? s : 0;
            var input = inputQuantities.TryGetValue(productId, out var i) ? i : 0;
            var canceled = canceledQuantities.TryGetValue(productId, out var c) ? c : 0;
            var shippingTypeDict = soldByShippingTypeDict.TryGetValue(productId, out var sDict)
                ? sDict
                : new Dictionary<ShippingType, int>();

            return new ProductInventorySummaryModel
            {
                SKU = x.Product.SKU,
                Name = x.Product.Name,
                Unit = x.Product.Unit.ToString(),
                OriginalQuantity = original,
                BuyQuantity = sold,
                InputQuantity = input,
                Quantity = original + input - sold - canceled, // ✅ fix công thức
                SoldByShippingType = shippingTypeDict
            };
        }).ToList();

        // 7. Trả kết quả
        return new QueryResult<ProductInventorySummaryModel>
        {
            Data = summaries,
            TotalCount = totalCount
        };
    }
    #region logic GetProductsAccountingInventory cũ
    //public async Task<QueryResult<AccountingInventoryModel>> GetProductsAccountingInventoryAsync(
    //    Guid storeId,
    //    GetAccountingInventoryQuery getProductsAccountingInventoryQuery,
    //    QueryInfo queryInfo)
    //{
    //    var startDate = getProductsAccountingInventoryQuery.FromDate;
    //    var endDate = getProductsAccountingInventoryQuery.ToDate.AddDays(1);

    //    // --- 1️⃣ Load dữ liệu InventoryPeriodLocks trước (client side chọn bản mới nhất) ---
    //    var iplAll = await _context.InventoryPeriodLocks
    //        .AsNoTracking()
    //        .Where(x => x.StoreId == storeId)
    //        .ToListAsync();

    //    var iplFromDateDict = iplAll
    //        .Where(x => x.CreateDate <= startDate && x.ProductId.HasValue)
    //        .GroupBy(x => x.ProductId!.Value)
    //        .ToDictionary(
    //            g => g.Key,
    //            g => g.OrderByDescending(x => x.CreateDate).FirstOrDefault()
    //        );

    //    var iplToDateDict = iplAll
    //        .Where(x => x.CreateDate <= endDate && x.ProductId.HasValue)
    //        .GroupBy(x => x.ProductId!.Value)
    //        .ToDictionary(
    //            g => g.Key,
    //            g => g.OrderByDescending(x => x.CreateDate).FirstOrDefault()
    //        );

    //    // --- 2️⃣ Load Inventory + Product ---
    //    var baseQuery = from i in _context.Inventories.AsNoTracking()
    //                    join p in _context.Products.AsNoTracking()
    //                        on i.ProductId equals p.Id
    //                    where i.StoreId == storeId &&
    //                          (queryInfo.SearchText == null ||
    //                           EF.Functions.Like(p.SKU, $"%{queryInfo.SearchText}%") ||
    //                           EF.Functions.Like(p.Name, $"%{queryInfo.SearchText}%"))
    //                    orderby p.SKU
    //                    select new { i, p };

    //    // --- 3️⃣ Count trước (đếm ở SQL, không liên quan subquery) ---
    //    int totalCount = queryInfo.NeedTotalCount
    //        ? await baseQuery.CountAsync()
    //        : 0;

    //    // --- 4️⃣ Phân trang và lấy danh sách sản phẩm ---
    //    var pageData = await baseQuery
    //        .Skip(queryInfo.Skip)
    //        .Take(queryInfo.Top)
    //        .ToListAsync();

    //    // --- 5️⃣ Load input / output ---
    //    var productIds = pageData.Select(x => x.p.Id).ToList();

    //    var inputData = await _context.DocumentSuppliers
    //        .AsNoTracking()
    //        .Where(ds =>
    //            ds.StoreId == storeId &&
    //            productIds.Contains(ds.ProductId!.Value) &&
    //            (ds.TransType == TransType.GoodsReceipt || ds.TransType == TransType.ReturnOutput) &&
    //            ds.ModifiedDate >= startDate &&
    //            ds.ModifiedDate < endDate)
    //        .GroupBy(ds => new { ds.ProductId, ds.TransType })
    //        .Select(g => new
    //        {
    //            g.Key.ProductId,
    //            g.Key.TransType,
    //            Quantity = g.Sum(x => x.Quantity)
    //        })
    //        .ToListAsync();

    //    var outputData = await _context.DocumentSuppliers
    //        .AsNoTracking()
    //        .Where(ds =>
    //            ds.StoreId == storeId &&
    //            productIds.Contains(ds.ProductId!.Value) &&
    //            (ds.TransType == TransType.ReturnedGoodsReceipt || ds.TransType == TransType.CanceledProduct) &&
    //            ds.ModifiedDate >= startDate &&
    //            ds.ModifiedDate < endDate)
    //        .GroupBy(ds => new { ds.ProductId, ds.TransType })
    //        .Select(g => new
    //        {
    //            g.Key.ProductId,
    //            g.Key.TransType,
    //            Quantity = g.Sum(x => x.Quantity)
    //        })
    //        .ToListAsync();

    //    // --- 6️⃣ Mapping kết quả ---
    //    var result = pageData.Select(x =>
    //    {
    //        var fromLock = iplFromDateDict.GetValueOrDefault(x.p.Id);
    //        var toLock = iplToDateDict.GetValueOrDefault(x.p.Id);

    //        var input = inputData
    //            .Where(d => d.ProductId == x.p.Id)
    //            .Select(d => new AccountingDocumentModel
    //            {
    //                TransTypeName = d.TransType.ToString(),
    //                Quantity = d.Quantity
    //            })
    //            .ToList();

    //        var output = outputData
    //            .Where(d => d.ProductId == x.p.Id)
    //            .Select(d => new AccountingDocumentModel
    //            {
    //                TransTypeName = d.TransType.ToString(),
    //                Quantity = d.Quantity
    //            })
    //            .ToList();

    //        return new AccountingInventoryModel
    //        {
    //            InventoryId = x.i.Id,
    //            ProductId = x.p.Id,
    //            Code = x.p.SKU,
    //            Name = x.p.Name,
    //            Unit = x.p.Unit.ToString(),
    //            InitialQuantity = fromLock?.OpeningQuantity ?? 0,
    //            RealQuantity = toLock?.OpeningQuantity ?? 0,
    //            CurrentQuantity = x.i.Quantity,
    //            InputQuantity = input,
    //            OutputQuantity = output,
    //            IsActive = x.p.IsActive
    //        };
    //    }).ToList();

    //    // --- 7️⃣ Trả về ---
    //    return new QueryResult<AccountingInventoryModel>
    //    {
    //        Data = result,
    //        TotalCount = totalCount
    //    };
    //}
    #endregion
    public async Task<QueryResult<AccountingInventoryModel>> GetProductsAccountingInventoryAsync(
     Guid storeId,
     DateTime fromDate,
     DateTime toDate,
     QueryInfo queryInfo)
    {
        var fromDateStart = fromDate.Date;
        var toDateEnd = toDate.Date.AddDays(1);
        var today = DateTime.UtcNow.Date;

        // 1️⃣ Lấy danh sách productId của store + join Products
        var productQuery = from i in _context.Inventories.AsNoTracking()
                           join p in _context.Products.AsNoTracking() on i.ProductId equals p.Id
                           where i.StoreId == storeId
                           select new
                           {
                               i.ProductId,
                               i.Quantity,
                               p.SKU,
                               p.Name
                           };

        // Search
        if (!string.IsNullOrEmpty(queryInfo.SearchText))
        {
            var keyword = queryInfo.SearchText.ToLower();
            productQuery = productQuery.Where(x =>
                x.SKU.ToLower().Contains(keyword) ||
                x.Name.ToLower().Contains(keyword));
        }

        var totalCount = await productQuery.CountAsync();

        // Paging
        var pagedProducts = await productQuery
            .OrderBy(x => x.Name)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        var productIds = pagedProducts.Select(x => x.ProductId).Distinct().ToList();

        // 2️⃣ Tồn đầu (ClosingQuantity gần nhất trước fromDate)
        var openingLocks = await _context.InventoryPeriodLocks
            .AsNoTracking()
            .Where(x => x.StoreId == storeId &&
                        x.ProductId.HasValue &&
                        x.CreateDate < fromDateStart &&
                        productIds.Contains(x.ProductId.Value))
            .GroupBy(x => x.ProductId!.Value)
            .Select(g => g.OrderByDescending(x => x.CreateDate).FirstOrDefault())
            .ToListAsync();

        var openingDict = openingLocks
            .Where(x => x != null)
            .ToDictionary(x => x!.ProductId!.Value, x => x!.ClosingQuantity);

        // 3️⃣ Lấy giao dịch trong khoảng thời gian
        var ds = await _context.DocumentSuppliers
            .AsNoTracking()
            .Where(d =>
                d.ProductId.HasValue &&
                productIds.Contains(d.ProductId.Value) &&
                (
                    d.StoreId == storeId ||
                    (d.ToStoreId == storeId && d.TransType == TransType.Exportrotation)
                ) &&
                d.CreateDate >= fromDateStart &&
                d.CreateDate < toDateEnd)
            .ToListAsync();

        var grouped = ds.GroupBy(x => x.ProductId!.Value)
                        .ToDictionary(g => g.Key, g => g.ToList());

        // Lấy số lượng bán ra từ SalesOrderItem
        var soldDict = await _context.SalesOrderItems
            .AsNoTracking()
            .Where(si => productIds.Contains(si.ProductId))
            .Where(si => _context.SalesOrders
                .Any(so => so.Id == si.SalesOrderId &&
                           so.StoreId == storeId &&
                           so.CreateDate >= fromDateStart &&
                           so.CreateDate < toDateEnd))
            .GroupBy(si => si.ProductId)
            .ToDictionaryAsync(g => g.Key, g => g.Sum(si => (decimal)si.Quantity));

        // 4️⃣ Lấy tồn cuối
        Dictionary<Guid, decimal> closingDict = new();

        if (toDate.Date < today)
        {
            // Nếu toDate < hôm nay → lấy từ InventoryPeriodLock
            var closingLocks = await _context.InventoryPeriodLocks
                .AsNoTracking()
                .Where(x => x.StoreId == storeId &&
                            x.ProductId.HasValue &&
                            productIds.Contains(x.ProductId.Value) &&
                            x.CreateDate < toDateEnd)
                .GroupBy(x => x.ProductId!.Value)
                .Select(g => g.OrderByDescending(x => x.CreateDate).FirstOrDefault())
                .ToListAsync();

            closingDict = closingLocks
                .Where(x => x != null)
                .ToDictionary(x => x!.ProductId!.Value, x => x!.ClosingQuantity);
        }
        else
        {
            // Nếu toDate = hôm nay → lấy trực tiếp từ bảng Inventories
            closingDict = pagedProducts.ToDictionary(x => x.ProductId, x => x.Quantity);
        }

        // 5️⃣ Mapping kết quả
        var result = new List<AccountingInventoryModel>();

        foreach (var p in pagedProducts)
        {
            var productId = p.ProductId;
            var tonDau = openingDict.GetValueOrDefault(productId, 0);
            var tonCuoi = closingDict.GetValueOrDefault(productId, 0);

            var transList = grouped.GetValueOrDefault(productId, new List<DocumentSupplierEntity>());

            // Lấy danh sách chi tiết các lần điều chỉnh (TransType = 3) - sắp xếp cũ → mới
            var adjustments = transList
                .Where(x => x.StoreId == storeId && x.TransType == TransType.AdjustmentEntry)
                //.OrderBy(x => x.CreateDate)
                .Select(x => new AdjustmentDetailModel
                {
                    Quantity = x.Quantity,                    // giữ nguyên dấu âm/dương từ DB
                    Date = x.CreateDate,
                    DocumentName = x.Document?.DocumentName   // nếu muốn hiển thị tên chứng từ
                })
                .ToList();

            // --- Nhập ---
            var nhapHang = transList.Where(x => x.StoreId == storeId && x.TransType == TransType.GoodsReceipt).Sum(x => x.Quantity);
            var xuatTraLai = transList.Where(x => x.StoreId == storeId && x.TransType == TransType.ReturnOutput).Sum(x => x.Quantity);
            var nhapXuatChuyenDen = transList.Where(x => x.StoreId == storeId && x.TransType == TransType.Exportrotation).Sum(x => x.Quantity);

            var tongNhap = nhapHang + xuatTraLai + nhapXuatChuyenDen;

            // --- Xuất ---
            var xuatHoanKhoTong = transList.Where(x => x.StoreId == storeId && x.TransType == TransType.ReturnedGoodsReceipt).Sum(x => x.Quantity);
            var huySanPham = transList.Where(x => x.StoreId == storeId && x.TransType == TransType.CanceledProduct).Sum(x => x.Quantity);
            var xuatChuyenDen = transList.Where(x => x.ToStoreId == storeId && x.TransType == TransType.Exportrotation).Sum(x => x.Quantity);

            var tongXuat = xuatHoanKhoTong + huySanPham + xuatChuyenDen;

            // 🧮 Bán ra = Tồn đầu + Tổng nhập - Tổng xuất - Tồn cuối
            //var soldQuantity = tonDau + tongNhap - tongXuat - tonCuoi;
            var soldQuantity = soldDict.GetValueOrDefault(productId, 0m);

            result.Add(new AccountingInventoryModel
            {
                ProductId = productId,
                ProductSku = p.SKU,
                ProductName = p.Name,
                InitialQuantity = tonDau,
                RealQuantity = tonCuoi,
                SoldQuantity = soldQuantity,
                InputQuantity = new List<AccountingDocumentModel>
            {
                new() { TransTypeName = "Nhập hàng", Quantity = nhapHang },
                new() { TransTypeName = "Xuất trả lại", Quantity = xuatTraLai },
                new() { TransTypeName = "Nhập luân chuyển", Quantity = nhapXuatChuyenDen }
                },
                OutputQuantity = new List<AccountingDocumentModel>
            {
                new() { TransTypeName = "Xuất hoàn kho tổng", Quantity = xuatHoanKhoTong },
                new() { TransTypeName = "Hủy sản phẩm", Quantity = huySanPham },
                new() { TransTypeName = "Xuất luân chuyển", Quantity = xuatChuyenDen },
                new() { TransTypeName = "Bán ra", Quantity = soldQuantity }
            },
                Adjustments = adjustments
            });
        }

        return new QueryResult<AccountingInventoryModel>
        {
            Data = result,
            TotalCount = totalCount
        };
    }

    #region logic GetIngredientsAccountingInventory cũ
    //public async Task<QueryResult<AccountingIngredientInventoryModel>> GetIngredientsAccountingInventoryAsync
    //    (Guid storeId, GetAccountingInventoryQuery getAccountingInventoryQuery, QueryInfo queryInfo)
    //{
    //    var startDate = getAccountingInventoryQuery.FromDate;
    //    var endDate = getAccountingInventoryQuery.ToDate.AddDays(1);

    //    // --- 1️⃣ Load dữ liệu InventoryPeriodLocks trước (client side chọn bản mới nhất) ---
    //    var iplAll = await _context.InventoryPeriodLocks
    //        .AsNoTracking()
    //        .Where(x => x.StoreId == storeId)
    //        .ToListAsync();

    //    var iplFromDateDict = iplAll
    //        .Where(x => x.CreateDate <= startDate && x.IngredientId.HasValue)
    //        .GroupBy(x => x.IngredientId!.Value)
    //        .ToDictionary(
    //            g => g.Key,
    //            g => g.OrderByDescending(x => x.CreateDate).FirstOrDefault()
    //        );

    //    var iplToDateDict = iplAll
    //        .Where(x => x.CreateDate <= endDate && x.IngredientId.HasValue)
    //        .GroupBy(x => x.IngredientId!.Value)
    //        .ToDictionary(
    //            g => g.Key,
    //            g => g.OrderByDescending(x => x.CreateDate).FirstOrDefault()
    //        );

    //    // --- 2️⃣ Load Inventory + Product ---
    //    var baseQuery = from i in _context.IngredientInventories.AsNoTracking()
    //                    join p in _context.Ingredients.AsNoTracking()
    //                        on i.IngredientId equals p.Id
    //                    where i.StoreId == storeId &&
    //                          (queryInfo.SearchText == null ||
    //                           EF.Functions.Like(p.Code, $"%{queryInfo.SearchText}%") ||
    //                           EF.Functions.Like(p.Name, $"%{queryInfo.SearchText}%"))
    //                    orderby p.Code
    //                    select new { i, p };

    //    // --- 3️⃣ Count trước (đếm ở SQL, không liên quan subquery) ---
    //    int totalCount = queryInfo.NeedTotalCount
    //        ? await baseQuery.CountAsync()
    //        : 0;

    //    // --- 4️⃣ Phân trang và lấy danh sách sản phẩm ---
    //    var pageData = await baseQuery
    //        .Skip(queryInfo.Skip)
    //        .Take(queryInfo.Top)
    //        .ToListAsync();

    //    // --- 5️⃣ Load input / output ---
    //    var ingredientIds = pageData.Select(x => x.p.Id).ToList();

    //    var inputData = await _context.DocumentSuppliers
    //        .AsNoTracking()
    //        .Where(ds =>
    //            ds.StoreId == storeId &&
    //            ingredientIds.Contains(ds.IngredientId!.Value) &&
    //            (ds.TransType == TransType.GoodsReceipt || ds.TransType == TransType.ReturnOutput) &&
    //            ds.ModifiedDate >= startDate &&
    //            ds.ModifiedDate < endDate)
    //        .GroupBy(ds => new { ds.ProductId, ds.TransType })
    //        .Select(g => new
    //        {
    //            g.Key.ProductId,
    //            g.Key.TransType,
    //            Quantity = g.Sum(x => x.Quantity)
    //        })
    //        .ToListAsync();

    //    var outputData = await _context.DocumentSuppliers
    //        .AsNoTracking()
    //        .Where(ds =>
    //            ds.StoreId == storeId &&
    //            ingredientIds.Contains(ds.IngredientId!.Value) &&
    //            (ds.TransType == TransType.ReturnedGoodsReceipt || ds.TransType == TransType.CanceledProduct) &&
    //            ds.ModifiedDate >= startDate &&
    //            ds.ModifiedDate < endDate)
    //        .GroupBy(ds => new { ds.ProductId, ds.TransType })
    //        .Select(g => new
    //        {
    //            g.Key.ProductId,
    //            g.Key.TransType,
    //            Quantity = g.Sum(x => x.Quantity)
    //        })
    //        .ToListAsync();

    //    // --- 6️⃣ Mapping kết quả ---
    //    var result = pageData.Select(x =>
    //    {
    //        var fromLock = iplFromDateDict.GetValueOrDefault(x.p.Id);
    //        var toLock = iplToDateDict.GetValueOrDefault(x.p.Id);

    //        var input = inputData
    //            .Where(d => d.ProductId == x.p.Id)
    //            .Select(d => new AccountingDocumentModel
    //            {
    //                TransTypeName = d.TransType.ToString(),
    //                Quantity = d.Quantity
    //            })
    //            .ToList();

    //        var output = outputData
    //            .Where(d => d.ProductId == x.p.Id)
    //            .Select(d => new AccountingDocumentModel
    //            {
    //                TransTypeName = d.TransType.ToString(),
    //                Quantity = d.Quantity
    //            })
    //            .ToList();

    //        return new AccountingIngredientInventoryModel
    //        {
    //            IngredientInventoryId = x.i.Id,
    //            IngredientId = x.p.Id,
    //            Code = x.p.Code,
    //            Name = x.p.Name,
    //            Unit = x.p.Unit.ToString(),
    //            InitialQuantity = fromLock?.OpeningQuantity ?? 0,
    //            RealQuantity = toLock?.OpeningQuantity ?? 0,
    //            CurrentQuantity = x.i.Quantity,
    //            InputQuantity = input,
    //            OutputQuantity = output,
    //        };
    //    }).ToList();

    //    // --- 7️⃣ Trả về ---
    //    return new QueryResult<AccountingIngredientInventoryModel>
    //    {
    //        Data = result,
    //        TotalCount = totalCount
    //    };
    //}
    #endregion

    public async Task<QueryResult<AccountingInventoryModel>> GetIngredientsAccountingInventoryAsync(
    Guid storeId,
    DateTime fromDate,
    DateTime toDate,
    QueryInfo queryInfo)
    {
        var fromDateStart = fromDate.Date;
        var toDateEnd = toDate.Date.AddDays(1);
        var today = DateTime.UtcNow.Date;

        // 1️⃣ Lấy danh sách IngredientId + join Ingredients
        var ingredientQuery =
            from i in _context.IngredientInventories.AsNoTracking()
            join ing in _context.Ingredients.AsNoTracking()
                on i.IngredientId equals ing.Id
            where i.StoreId == storeId
            select new
            {
                i.IngredientId,
                i.Quantity,
                ing.Name,
                ing.Code
            };

        // Search
        if (!string.IsNullOrEmpty(queryInfo.SearchText))
        {
            var keyword = queryInfo.SearchText.ToLower();
            ingredientQuery = ingredientQuery.Where(x =>
                x.Name.ToLower().Contains(keyword) ||
                x.Code.ToLower().Contains(keyword));
        }

        var totalCount = await ingredientQuery.CountAsync();

        // Paging
        var pagedIngredients = await ingredientQuery
            .OrderBy(x => x.Name)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        var ingredientIds = pagedIngredients.Select(x => x.IngredientId).Distinct().ToList();

        // 2️⃣ Tồn đầu (ClosingQuantity gần nhất trước fromDate)
        var openingLocks = await _context.InventoryPeriodLocks
            .AsNoTracking()
            .Where(x =>
                x.StoreId == storeId &&
                x.IngredientId.HasValue &&
                x.CreateDate < fromDateStart &&
                ingredientIds.Contains(x.IngredientId.Value))
            .GroupBy(x => x.IngredientId!.Value)
            .Select(g => g.OrderByDescending(x => x.CreateDate).FirstOrDefault())
            .ToListAsync();

        var openingDict = openingLocks
            .Where(x => x != null)
            .ToDictionary(x => x!.IngredientId!.Value, x => x!.ClosingQuantity);

        // 3️⃣ Load full giao dịch trong khoảng ngày
        var ds = await _context.DocumentSuppliers
            .AsNoTracking()
            .Where(d =>
                d.IngredientId.HasValue &&
                ingredientIds.Contains(d.IngredientId.Value) &&
                (
                    d.StoreId == storeId ||
                    (d.ToStoreId == storeId && d.TransType == TransType.Exportrotation)
                ) &&
                d.CreateDate >= fromDateStart &&
                d.CreateDate < toDateEnd)
            .ToListAsync();

        var grouped = ds
            .GroupBy(x => x.IngredientId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        // 4️⃣ Mapping tổng hợp kết quả
        var result = new List<AccountingInventoryModel>();

        foreach (var ing in pagedIngredients)
        {
            var ingredientId = ing.IngredientId;

            // --- Tồn đầu ---
            var tonDau = openingDict.GetValueOrDefault(ingredientId, 0);

            // --- Tồn cuối ---
            decimal tonCuoi;

            if (toDate.Date == today)
            {
                // Lấy tồn cuối hiện tại từ IngredientInventories
                tonCuoi = ing.Quantity;
            }
            else
            {
                // Lấy từ InventoryPeriodLocks
                var closingLock = await _context.InventoryPeriodLocks
                    .AsNoTracking()
                    .Where(x =>
                        x.StoreId == storeId &&
                        x.IngredientId == ingredientId &&
                        x.CreateDate <= toDateEnd)
                    .OrderByDescending(x => x.CreateDate)
                    .FirstOrDefaultAsync();

                tonCuoi = closingLock?.ClosingQuantity ?? 0;
            }

            var transList = grouped.GetValueOrDefault(ingredientId, new List<DocumentSupplierEntity>());

            // Lấy danh sách chi tiết các lần điều chỉnh (TransType = 3) - sắp xếp cũ → mới
            var adjustments = transList
                .Where(x => x.StoreId == storeId && x.TransType == TransType.AdjustmentEntry)
                //.OrderBy(x => x.CreateDate)
                .Select(x => new AdjustmentDetailModel
                {
                    Quantity = x.Quantity,                    // giữ nguyên dấu âm/dương từ DB
                    Date = x.CreateDate,
                    DocumentName = x.Document?.DocumentName   // nếu muốn hiển thị tên chứng từ
                })
                .ToList();

            // --- Nhập ---
            var nhapHang = transList.Where(x => x.StoreId == storeId && x.TransType == TransType.GoodsReceipt).Sum(x => x.Quantity);
            var xuatTraLai = transList.Where(x => x.StoreId == storeId && x.TransType == TransType.ReturnOutput).Sum(x => x.Quantity);
            var nhapXuatChuyenDen = transList.Where(x => x.StoreId == storeId && x.TransType == TransType.Exportrotation).Sum(x => x.Quantity);

            var tongNhap = nhapHang + xuatTraLai + nhapXuatChuyenDen;

            // --- Xuất ---
            var xuatHoanKhoTong = transList.Where(x => x.StoreId == storeId && x.TransType == TransType.ReturnedGoodsReceipt).Sum(x => x.Quantity);
            var huySanPham = transList.Where(x => x.StoreId == storeId && x.TransType == TransType.CanceledProduct).Sum(x => x.Quantity);
            var xuatChuyenDen = transList.Where(x => x.ToStoreId == storeId && x.TransType == TransType.Exportrotation).Sum(x => x.Quantity);

            var tongXuat = xuatHoanKhoTong + huySanPham + xuatChuyenDen;

            // 🧮 Đã dùng = Tồn đầu + Tổng nhập − Tổng xuất − Tồn cuối
            var soldQuantity = tonDau + tongNhap - tongXuat - tonCuoi;

            // Bảo đảm không có giá trị null → ở đây đều là số, không null

            result.Add(new AccountingInventoryModel
            {
                InventoryId = ingredientId,
                IngredientCode = ing.Code,
                IngredientName = ing.Name,

                InitialQuantity = tonDau,
                RealQuantity = tonCuoi,
                SoldQuantity = soldQuantity,

                InputQuantity = new List<AccountingDocumentModel>
            {
                new() { TransTypeName = "Nhập hàng", Quantity = nhapHang },
                new() { TransTypeName = "Xuất trả lại", Quantity = xuatTraLai },
                new() { TransTypeName = "Nhập luân chuyển", Quantity = nhapXuatChuyenDen }
            },

                OutputQuantity = new List<AccountingDocumentModel>
            {
                new() { TransTypeName = "Nhập hoàn kho", Quantity = xuatHoanKhoTong },
                new() { TransTypeName = "Hủy sản phẩm", Quantity = huySanPham },
                new() { TransTypeName = "Xuất luân chuyển", Quantity = xuatChuyenDen },
                new() { TransTypeName = "Đã dùng", Quantity = soldQuantity }
            },
                Adjustments = adjustments

            });
        }

        return new QueryResult<AccountingInventoryModel>
        {
            Data = result,
            TotalCount = totalCount
        };
    }

    public async Task<QueryResult<DocumentReportByTransType>> GetDocumentReportByTranTypeAsync
        (Guid storeId, DocumentReportByTransTypeQuery documentReportByTransTypeQuery, QueryInfo queryInfo)
    {
        TransType transTypeStaff = default;
        var hasTransTypeStaff = !string.IsNullOrEmpty(documentReportByTransTypeQuery.TransType) &&
                              Enum.TryParse(documentReportByTransTypeQuery.TransType, out transTypeStaff);
        var query = from document in _context.Documents.AsNoTracking()
                    join documentSupplier in _context.DocumentSuppliers.AsNoTracking()
                        on document.Id equals documentSupplier.DocumentId
                    join product in _context.Products.AsNoTracking()
                        on documentSupplier.ProductId equals product.Id
                    where documentSupplier.StoreId == storeId &&
                            EF.Functions.Collate(product.Name, "Latin1_General_CI_AI")
                              .Contains(queryInfo.SearchText ?? "") &&
                        documentSupplier.TransType == transTypeStaff &&
                        document.ModifiedDate >= documentReportByTransTypeQuery.FromDate &&
                        document.ModifiedDate < documentReportByTransTypeQuery.ToDate
                    orderby document.CreateDate descending

                    select new DocumentReportByTransType
                    {
                        DocumentName = document.DocumentName,
                        ProductName = product.Name,
                        Date = document.CreateDate.ToString("dd-MM-yyyy"),
                        Time = document.CreateDate.ToString("HH:mm"),
                        Quantity = documentSupplier.Quantity,
                        Unit = product.Unit,
                    };

        var pagedEntities = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        var totalCount = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;
        return new QueryResult<DocumentReportByTransType>
        {
            Data = pagedEntities,
            TotalCount = totalCount
        };
    }
    private async Task<TotalReturnResult> CaculateTotalMoneyReturn(DateTime date, DateTime nextDate, Guid? storeId)
    {
        var latestSalesOrderPerProduct = from soi in _context.SalesOrderItems
                                         where soi.SalesOrder != null &&
                                               soi.SalesOrder.OrderStatus == OrderStatusType.Confirmed &&
                                               soi.SalesOrder.ModifiedDate >= date &&
                                               soi.SalesOrder.ModifiedDate < nextDate
                                         group soi by soi.ProductId into g
                                         select new
                                         {
                                             ProductId = g.Key,
                                             MaxDate = g.Max(x => x.SalesOrder!.ModifiedDate)
                                         };

        var latestSalesOrderItems = from soi in _context.SalesOrderItems
                                    join latest in latestSalesOrderPerProduct
                                      on new { soi.ProductId, soi.SalesOrder!.ModifiedDate }
                                      equals new { latest.ProductId, ModifiedDate = latest.MaxDate }
                                    select new
                                    {
                                        soi.ProductId,
                                        soi.UnitPrice,
                                    };

        var totalMoneyReturnQuery = from doc in _context.DocumentSuppliers
                                    join soi in latestSalesOrderItems
                                      on doc.ProductId equals soi.ProductId
                                    where doc.TransType == TransType.ReturnOutput &&
                                          doc.StoreId == storeId &&
                                          doc.ModifiedDate >= date &&
                                          doc.ModifiedDate < nextDate
                                    select new
                                    {
                                        Money = doc.Quantity * soi.UnitPrice,
                                        Quantity = doc.Quantity
                                    };

        var resultList = await totalMoneyReturnQuery.ToListAsync();

        return new TotalReturnResult
        {
            TotalMoney = resultList.Sum(x => x.Money),
            TotalQuantity = (int)resultList.Sum(x => x.Quantity)
        };
    }

    public async Task<List<TopStoreRevenueModel>> GetTopRevenseInStoresAsync(DashboardQuery query)
    {
        var orderQuery = _context.SalesOrders.AsNoTracking()
                        .Join(_context.SalesOrderItems.AsNoTracking(),
                            order => order.Id,
                            item => item.SalesOrderId,
                            (order, item) => new { order, item })
                        .Where(o => o.order.OrderStatus == OrderStatusType.Confirmed &&
                                (o.order.ModifiedDate >= query.FromDate && o.order.ModifiedDate < query.ToDate));

        var storeOrders = from order in orderQuery
                          join store in _context.Stores on order.order.StoreId equals store.Id
                          select new
                          {
                              store.Id,
                              store.Name,
                              order.order.PaidAmount
                          };

        var result = await storeOrders
                        .GroupBy(x => new { x.Id, x.Name })
                        .Select(g => new TopStoreRevenueModel
                        {
                            StoreId = g.Key.Id,
                            StoreName = g.Key.Name,
                            TotalOrder = g.Count(),
                            TotalRevenue = g.Sum(x => x.PaidAmount)
                        })
                        .OrderByDescending(x => x.TotalRevenue)
                        .Take(5)
                        .ToListAsync();

        return result;
    }

    public async Task<Dictionary<string, decimal>> GetTopPaymentMethodInStoresAsync(DashboardQuery query)
    {
        var queryable = _context.SalesOrders.AsNoTracking()
                        .Where(o => o.OrderStatus == OrderStatusType.Confirmed
                                    && o.ModifiedDate >= query.FromDate
                                    && o.ModifiedDate < query.ToDate);

        if (query.StoreId.HasValue)
        {
            queryable = queryable.Where(o => o.StoreId == query.StoreId.Value);
        }

        var grouped = await queryable
            .GroupBy(o => o.PaymentMethod)
            .Select(g => new
            {
                PaymentMethod = g.Key.ToString()!,
                TotalAmount = g.Sum(x => x.PaidAmount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToListAsync();


        var grandTotal = grouped.Sum(x => x.TotalAmount);

        var result = grouped.ToDictionary(
            x => x.PaymentMethod,
            x => grandTotal == 0 ? 0 : Math.Round((x.TotalAmount / grandTotal) * 100, 2)
        );

        return result;
    }

    public IAsyncEnumerable<TopStoreRevenueModel> GetRevenseInStoresAsync(DashboardQuery query)
    {
        return _context.SalesOrders
            .AsNoTracking()
            .Where(o => o.OrderStatus == OrderStatusType.Confirmed
                        && o.ModifiedDate >= query.FromDate
                        && o.ModifiedDate < query.ToDate)
            .GroupBy(o => o.StoreId)
            .Select(g => new TopStoreRevenueModel
            {
                StoreId = g.Key,
                StoreName = g.FirstOrDefault()!.Store!.Name,
                TotalOrder = g.Count(),
                TotalRevenue = g.Sum(x => x.PaidAmount)
            })
            .OrderByDescending(x => x.TotalRevenue)
            .AsAsyncEnumerable();
    }

    public record TotalReturnResult
    {
        public decimal TotalMoney { get; set; }
        public int TotalQuantity { get; set; }
    }

    //public async Task<SalesReportModel> GetSalesReportAsync(QueryInfo queryInfo, GetSalesReportQuery query)
    //{
    //    // 1. Chuẩn bị khoảng thời gian lọc (bao gồm cả ngày ToDate)
    //    var fromDate = query.FromDate.Date;
    //    var toDate = query.ToDate.Date.AddDays(1);

    //    // 2. Lấy tất cả đơn hàng đã xác nhận (Confirmed) trong khoảng thời gian
    //    var ordersQuery = _context.SalesOrders
    //        .AsNoTracking()
    //        .Where(o => o.OrderStatus == OrderStatusType.Confirmed &&
    //                    o.ModifiedDate >= fromDate &&
    //                    o.ModifiedDate < toDate);

    //    // Lọc theo cửa hàng nếu có truyền StoreId
    //    if (query.StoreId.HasValue)
    //    {
    //        ordersQuery = ordersQuery.Where(o => o.StoreId == query.StoreId.Value);
    //    }

    //    // 3. Join với SalesOrderItems và Products để lấy chi tiết sản phẩm
    //    // Đồng thời áp dụng filter: sản phẩm còn hoạt động (IsActive) theo QueryInfo
    //    var rawDataQuery = from order in ordersQuery
    //                       join item in _context.SalesOrderItems.AsNoTracking()
    //                           on order.Id equals item.SalesOrderId
    //                       join product in _context.Products.AsNoTracking()
    //                           on item.ProductId equals product.Id

    //                       // Lấy tất cả promotion của đơn hàng này (chỉ 1 lần)
    //                       let allPromotions = _context.SalesOrderPromotions
    //                           .AsNoTracking()
    //                           .Where(p => p.SalesOrderId == order.Id).ToList()

    //                       // Giảm giá gắn trực tiếp vào item (có thể có nhiều promotion → Sum lại)
    //                       let itemSpecificDiscount = allPromotions
    //                           .Where(p => p.SalesOrderItemId == item.Id)
    //                           .Sum(p => p.DiscountAmount)

    //                       // Giảm giá cấp đơn hàng (SalesOrderItemId = null → cần phân bổ)
    //                       let orderLevelDiscount = allPromotions
    //                           .Where(p => p.SalesOrderItemId == null)
    //                           .Sum(p => p.DiscountAmount)

    //                       where product.IsActive == queryInfo.IsActive // Lọc sản phẩm active/inactive
    //                       select new
    //                       {
    //                           OrderId = order.Id,
    //                           OrderTotalAmount = order.TotalAmount,     // Tổng tiền đơn hàng (trước giảm giá)
    //                           //OrderDiscount = order.Discount,           // Tổng giảm giá của đơn hàng
    //                           ItemSpecificDiscount = itemSpecificDiscount,    // Giảm giá gắn item
    //                           OrderLevelDiscount = orderLevelDiscount,        // Giảm giá cấp đơn (phân bổ sau)
    //                           ProductId = item.ProductId,
    //                           ProductGroupId = product.ProductGroupId,
    //                           product.SKU,
    //                           product.Name,
    //                           product.Price,                            // Giá bán gốc của sản phẩm
    //                           item.Quantity,
    //                           item.UnitPrice,                           // Giá bán thực tế trên item (có thể thay đổi)
    //                           item.TotalPrice                           // Quantity × UnitPrice (trước giảm giá)
    //                       };

    //    // Lọc theo nhóm sản phẩm nếu có
    //    if (query.ProductGroupId.HasValue)
    //    {
    //        rawDataQuery = rawDataQuery.Where(x => x.ProductGroupId == query.ProductGroupId.Value);
    //    }

    //    // Lọc theo sản phẩm cụ thể nếu có
    //    if (query.ProductId.HasValue)
    //    {
    //        rawDataQuery = rawDataQuery.Where(x => x.ProductId == query.ProductId.Value);
    //    }

    //    // 4. Tìm kiếm theo từ khóa (SKU hoặc Tên sản phẩm) - không phân biệt hoa thường
    //    if (!string.IsNullOrEmpty(queryInfo.SearchText))
    //    {
    //        var searchText = $"%{queryInfo.SearchText}%";
    //        rawDataQuery = rawDataQuery.Where(x =>
    //            EF.Functions.Collate(x.SKU, "Latin1_General_CI_AI").Contains(searchText) ||
    //            EF.Functions.Collate(x.Name, "Latin1_General_CI_AI").Contains(searchText));
    //    }

    //    // 5. Thực thi query và tải toàn bộ dữ liệu thô về bộ nhớ
    //    // (Cần client-side để phân bổ discount chính xác)
    //    var rawData = await rawDataQuery.ToListAsync();

    //    // 6. Phân bổ giảm giá từ cấp đơn hàng xuống từng sản phẩm (theo tỷ lệ đóng góp)
    //    // Công thức: Discount của item = (TotalPrice của item / Tổng tiền đơn) × Discount đơn hàng
    //    var itemDetails = rawData.Select(x =>
    //    {
    //        decimal finalDiscount = x.ItemSpecificDiscount; // Ưu tiên discount gắn thẳng vào item

    //        // Nếu không có discount gắn item → phân bổ từ discount cấp đơn hàng
    //        if (finalDiscount == 0m && x.OrderLevelDiscount > 0m && x.OrderTotalAmount > 0m)
    //        {
    //            decimal ratio = x.TotalPrice / x.OrderTotalAmount;
    //            finalDiscount = Math.Round(x.OrderLevelDiscount * ratio, 2);
    //        }

    //        decimal itemNetAmount = x.TotalPrice - finalDiscount;

    //        return new
    //        {
    //            x.ProductId,
    //            x.SKU,
    //            x.Name,
    //            x.Price,
    //            x.Quantity,
    //            ItemDiscount = finalDiscount,
    //            ItemNetAmount = itemNetAmount
    //        };
    //        //decimal itemRatio = x.OrderTotalAmount > 0
    //        //    ? x.TotalPrice / x.OrderTotalAmount
    //        //    : 0m;

    //        //decimal itemDiscount = Math.Round(x.OrderDiscount * itemRatio, 2); // Làm tròn 2 chữ số
    //        //decimal itemNetAmount = x.TotalPrice - itemDiscount;             // Thành tiền sau giảm giá

    //        //return new
    //        //{
    //        //    x.ProductId,
    //        //    x.SKU,
    //        //    x.Name,
    //        //    x.Price,
    //        //    x.Quantity,
    //        //    ItemDiscount = itemDiscount,
    //        //    ItemNetAmount = itemNetAmount
    //        //};
    //    }).ToList();

    //    // 7. Gom nhóm theo sản phẩm để tính tổng số lượng bán, giảm giá, thành tiền
    //    var groupedProducts = itemDetails
    //        .GroupBy(x => new { x.ProductId, x.SKU, x.Name, x.Price })
    //        .Select(g => new SalesReportDetailModel
    //        {
    //            SKU = g.Key.SKU,
    //            Name = g.Key.Name,
    //            Price = g.Key.Price,
    //            QuantitySold = g.Sum(x => x.Quantity),
    //            TotalDiscount = g.Sum(x => x.ItemDiscount),
    //            //NetAmount = g.Sum(x => x.ItemNetAmount)
    //            NetAmount = g.Key.Price * g.Sum(x => x.Quantity) - g.Sum(x => x.ItemDiscount)
    //        })
    //        .ToList();

    //    // 8. Sắp xếp theo trường và thứ tự mà client yêu cầu
    //    groupedProducts = queryInfo.OrderBy switch
    //    {
    //        "Name" => queryInfo.OrderType == OrderType.Ascending
    //            ? groupedProducts.OrderBy(x => x.Name).ToList()
    //            : groupedProducts.OrderByDescending(x => x.Name).ToList(),

    //        "Price" => queryInfo.OrderType == OrderType.Ascending
    //            ? groupedProducts.OrderBy(x => x.Price).ToList()
    //            : groupedProducts.OrderByDescending(x => x.Price).ToList(),

    //        "QuantitySold" => queryInfo.OrderType == OrderType.Ascending
    //            ? groupedProducts.OrderBy(x => x.QuantitySold).ToList()
    //            : groupedProducts.OrderByDescending(x => x.QuantitySold).ToList(),

    //        "TotalDiscount" => queryInfo.OrderType == OrderType.Ascending
    //            ? groupedProducts.OrderBy(x => x.TotalDiscount).ToList()
    //            : groupedProducts.OrderByDescending(x => x.TotalDiscount).ToList(),

    //        "NetAmount" => queryInfo.OrderType == OrderType.Ascending
    //            ? groupedProducts.OrderBy(x => x.NetAmount).ToList()
    //            : groupedProducts.OrderByDescending(x => x.NetAmount).ToList(),

    //        // Mặc định sắp xếp theo SKU (nếu OrderBy không hợp lệ hoặc null)
    //        _ => queryInfo.OrderType == OrderType.Ascending
    //            ? groupedProducts.OrderBy(x => x.SKU).ToList()
    //            : groupedProducts.OrderByDescending(x => x.SKU).ToList()
    //    };

    //    // 9. Tính toán tổng hợp cho phần Summary (toàn bộ dữ liệu, không chỉ trang hiện tại)
    //    var totalQuantitySold = groupedProducts.Sum(x => x.QuantitySold);
    //    var totalOriginalAmount = groupedProducts.Sum(x => x.QuantitySold * x.Price); // Tổng giá gốc
    //    var totalDiscount = groupedProducts.Sum(x => x.TotalDiscount);
    //    var totalNetAmount = groupedProducts.Sum(x => x.NetAmount);
    //    //var totalNetAmount = totalOriginalAmount - totalDiscount;

    //    var summary = new SalesReportSummaryModel
    //    {
    //        TotalQuantitySold = totalQuantitySold,
    //        TotalOriginalAmount = totalOriginalAmount,
    //        TotalDiscount = totalDiscount,
    //        TotalNetAmount = totalNetAmount
    //    };

    //    // 10. Phân trang theo Skip & Top từ QueryInfo
    //    var totalCount = queryInfo.NeedTotalCount ? groupedProducts.Count : 0;

    //    var pagedProducts = groupedProducts
    //        .Skip(queryInfo.Skip)
    //        .Take(queryInfo.Top)
    //        .ToList();

    //    var productsResult = new QueryResult<SalesReportDetailModel>
    //    {
    //        Data = pagedProducts,
    //        TotalCount = totalCount
    //    };

    //    // 11. Trả về kết quả báo cáo
    //    return new SalesReportModel
    //    {
    //        Summary = summary,
    //        Products = productsResult
    //    };
    //}

    public async Task<SalesReportModel> GetSalesReportAsync(QueryInfo queryInfo, GetSalesReportQuery query)
    {
        var fromDate = query.FromDate.Date;
        var toDate = query.ToDate.Date.AddDays(1);

        var ordersQuery = _context.SalesOrders
            .AsNoTracking()
            .Where(o => o.OrderStatus == OrderStatusType.Confirmed &&
                         o.ModifiedDate >= fromDate &&
                         o.ModifiedDate < toDate);

        if (query.StoreId.HasValue)
            ordersQuery = ordersQuery.Where(o => o.StoreId == query.StoreId.Value);

        // === LỌC THEO SHIPPINGTYPE (chỉ đoạn này thay đổi) ===
        if (query.ShippingType.HasValue)
            ordersQuery = ordersQuery.Where(o => o.ShippingType == query.ShippingType.Value);

        // Lấy dữ liệu thô – chính xác từng đồng
        var rawData = await (
            from order in ordersQuery
            join item in _context.SalesOrderItems.AsNoTracking() on order.Id equals item.SalesOrderId
            join product in _context.Products.AsNoTracking() on item.ProductId equals product.Id
            join promo in _context.SalesOrderPromotions.AsNoTracking()
                on item.Id equals promo.SalesOrderItemId into promos
            from promo in promos.DefaultIfEmpty()
            where !query.ProductGroupId.HasValue || product.ProductGroupId == query.ProductGroupId.Value  // THÊM DÒNG NÀY
            select new
            {
                product.SKU,
                product.Name,

                // Giá thực tế khách thấy trên hóa đơn = giá sau giảm tay + promotion chia đều
                DisplayUnitPrice = item.UnitPrice + (promo != null ? promo.DiscountAmount / item.Quantity : 0m),

                // Giá đã lưu trong item (sau giảm tay, trước promotion)
                SavedUnitPrice = item.UnitPrice,

                item.Quantity,

                // Tiền trước mọi loại giảm giá (giá gốc thực tế)
                TotalBeforeDiscount = (item.UnitPrice + (promo != null ? promo.DiscountAmount / item.Quantity : 0m)) * item.Quantity,

                // Tiền dùng để phân bổ discount cấp đơn (lấy từ TotalPrice cũ)
                AllocatableTotalPrice = item.TotalPrice,

                ItemDiscountAmount = promo != null ? promo.DiscountAmount : 0m,
                OrderId = order.Id,
                OrderTotalAmount = order.TotalAmount
            }
        ).ToListAsync();

        // Lấy discount cấp đơn hàng (SalesOrderItemId = null)
        var orderLevelDiscounts = await _context.SalesOrderPromotions
            .AsNoTracking()
            .Where(p => p.SalesOrderItemId == null &&
                         ordersQuery.Select(o => o.Id).Contains(p.SalesOrderId))
            .GroupBy(p => p.SalesOrderId)
            .Select(g => new { OrderId = g.Key, Discount = g.Sum(p => p.DiscountAmount) })
            .ToDictionaryAsync(x => x.OrderId, x => x.Discount);

        // Gom nhóm theo SKU + Giá hiển thị (DisplayUnitPrice)
        var grouped = rawData
            .GroupBy(x => new { x.SKU, x.Name, x.DisplayUnitPrice })
            .Select(g =>
            {
                var totalItemDiscount = g.Sum(x => x.ItemDiscountAmount);
                var totalBeforeDiscount = g.Sum(x => x.TotalBeforeDiscount);
                var quantity = g.Sum(x => x.Quantity);

                // Phân bổ discount cấp đơn theo tỷ lệ tiền trước giảm (AllocatableTotalPrice)
                decimal allocatedOrderDiscount = 0m;
                var orderGroups = g.GroupBy(x => x.OrderId);
                foreach (var og in orderGroups)
                {
                    if (orderLevelDiscounts.TryGetValue(og.Key, out var orderDiscount) && orderDiscount > 0)
                    {
                        var orderAllocatableTotal = og.Sum(x => x.AllocatableTotalPrice);
                        if (orderAllocatableTotal > 0)
                        {
                            allocatedOrderDiscount += orderDiscount * (og.Sum(x => x.AllocatableTotalPrice) / orderAllocatableTotal);
                        }
                    }
                }

                var totalDiscount = totalItemDiscount + allocatedOrderDiscount;
                var netAmount = totalBeforeDiscount - totalDiscount;

                return new
                {
                    g.Key.SKU,
                    g.Key.Name,
                    UnitPrice = g.Key.DisplayUnitPrice,
                    QuantitySold = quantity,
                    TotalBeforeDiscount = totalBeforeDiscount,
                    TotalDiscount = Math.Round(totalDiscount, 2),
                    NetAmount = netAmount
                };
            })
            .GroupBy(x => new { x.SKU, x.Name })
            .Select(pg => new SalesReportDetailModel
            {
                SKU = pg.Key.SKU,
                Name = pg.Key.Name,
                PriceDetails = pg.Select(x => new SalesReportDetailItemModel
                {
                    UnitPrice = x.UnitPrice,
                    QuantitySold = x.QuantitySold,
                    TotalBeforeDiscount = x.TotalBeforeDiscount,
                    TotalDiscount = x.TotalDiscount,
                    NetAmount = x.NetAmount
                })
                .OrderByDescending(x => x.UnitPrice)
                .ToList()
            })
            .ToList();

        // Summary – đúng tuyệt đối
        var totalOriginalAmount = grouped.Sum(p => p.PriceDetails.Sum(d => d.TotalBeforeDiscount));
        var totalDiscount = grouped.Sum(p => p.PriceDetails.Sum(d => d.TotalDiscount));
        var totalNetAmount = totalOriginalAmount - totalDiscount; // chắc chắn đúng 100%

        var summary = new SalesReportSummaryModel
        {
            TotalQuantitySold = grouped.Sum(p => p.PriceDetails.Sum(d => d.QuantitySold)),
            TotalOriginalAmount = totalOriginalAmount,
            TotalDiscount = totalDiscount,
            TotalNetAmount = totalNetAmount
        };

        var paged = grouped
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToList();

        // Làm tròn tất cả số thập phân về 2 chữ số (chuẩn tiền tệ)
        var roundedData = paged.Select(p => new SalesReportDetailModel
        {
            SKU = p.SKU,
            Name = p.Name,
            PriceDetails = p.PriceDetails.Select(d => new SalesReportDetailItemModel
            {
                UnitPrice = Math.Round(d.UnitPrice, 2),
                QuantitySold = d.QuantitySold,
                TotalBeforeDiscount = Math.Round(d.TotalBeforeDiscount, 2),
                TotalDiscount = Math.Round(d.TotalDiscount, 2),
                NetAmount = Math.Round(d.NetAmount, 2)
            }).ToList()
        }).ToList();

        return new SalesReportModel
        {
            Summary = new SalesReportSummaryModel
            {
                TotalQuantitySold = summary.TotalQuantitySold,
                TotalOriginalAmount = Math.Round(summary.TotalOriginalAmount, 2),
                TotalDiscount = Math.Round(summary.TotalDiscount, 2),
                TotalNetAmount = Math.Round(summary.TotalNetAmount, 2)
            },
            Products = new QueryResult<SalesReportDetailModel>
            {
                Data = roundedData,
                TotalCount = queryInfo.NeedTotalCount ? grouped.Count : 0
            }
        };
    }

    public async Task<InventoryDetailReportModel> GetInventoryDetailReportAsync(
    QueryInfo queryInfo,
    GetInventoryDetailReportQuery query)
    {
        var fromDate = query.FromDate.Date;
        var toDateEnd = query.ToDate.Date.AddDays(1);
        var today = DateTime.UtcNow.Date;

        // 1. Lấy danh sách sản phẩm (phân trang + search + filter)
        var baseQuery = from i in _context.Inventories.AsNoTracking()
                        join p in _context.Products.AsNoTracking() on i.ProductId equals p.Id
                        where (query.StoreId == null || i.StoreId == query.StoreId.Value)
                              && p.IsActive == queryInfo.IsActive
                        select new
                        {
                            ProductId = i.ProductId,
                            StoreId = i.StoreId,
                            CurrentQuantity = i.Quantity,
                            p.SKU,
                            p.Name,
                            p.Unit,
                            p.ProductGroupId
                        };

        if (query.ProductGroupId.HasValue)
            baseQuery = baseQuery.Where(x => x.ProductGroupId == query.ProductGroupId.Value);

        if (!string.IsNullOrEmpty(queryInfo.SearchText))
        {
            var keyword = $"%{queryInfo.SearchText}%";
            baseQuery = baseQuery.Where(x =>
                EF.Functions.Collate(x.SKU, "Latin1_General_CI_AI").Contains(keyword) ||
                EF.Functions.Collate(x.Name, "Latin1_General_CI_AI").Contains(keyword));
        }

        var totalCount = queryInfo.NeedTotalCount ? await baseQuery.CountAsync() : 0;
        var pagedProducts = await baseQuery
            .OrderBy(x => x.SKU)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        var productIds = pagedProducts.Select(x => x.ProductId).ToList(); // ĐÃ SỬA: không dùng .Value
        if (!productIds.Any())
            return new InventoryDetailReportModel
            {
                Products = new QueryResult<InventoryDetailReportItemModel>
                {
                    Data = new List<InventoryDetailReportItemModel>(),
                    TotalCount = totalCount
                }
            };

        var storeId = query.StoreId ?? pagedProducts.First().StoreId;

        // 2. Tồn đầu kỳ
        var openingDict = await _context.InventoryPeriodLocks
            .AsNoTracking()
            .Where(x => x.StoreId == storeId
                        && x.ProductId.HasValue
                        && productIds.Contains(x.ProductId.Value)  // ĐÃ SỬA: dùng .Value vì ProductId là Guid?
                        && x.CreateDate < fromDate)
            .GroupBy(x => x.ProductId!.Value)
            .Select(g => g.OrderByDescending(x => x.CreateDate).First())
            .ToDictionaryAsync(x => x.ProductId!.Value, x => x.ClosingQuantity);

        // 3. Tồn cuối kỳ
        var closingDict = query.ToDate.Date >= today
            ? pagedProducts.ToDictionary(x => x.ProductId, x => x.CurrentQuantity)
            : await _context.InventoryPeriodLocks
                .AsNoTracking()
                .Where(x => x.StoreId == storeId
                            && x.ProductId.HasValue
                            && productIds.Contains(x.ProductId.Value)
                            && x.CreateDate < toDateEnd)
                .GroupBy(x => x.ProductId!.Value)
                .Select(g => g.OrderByDescending(x => x.CreateDate).First())
                .ToDictionaryAsync(x => x.ProductId!.Value, x => x.ClosingQuantity);

        // 4. Lấy chứng từ trong kỳ
        var docs = await _context.DocumentSuppliers
            .Include(x => x.Document) // ĐÃ SỬA: Include để lấy DocumentCode
            .AsNoTracking()
            .Where(d => d.ProductId.HasValue
                        && productIds.Contains(d.ProductId.Value)
                        && (d.StoreId == storeId || d.ToStoreId == storeId)
                        && d.CreateDate >= fromDate
                        && d.CreateDate < toDateEnd)
            .Select(d => new
            {
                ProductId = d.ProductId!.Value,
                d.TransType,
                d.Quantity,
                d.CreateDate,
                DocumentCode = d.Document != null ? (d.Document.DocumentName ?? d.Document.Id.ToString("N")) : d.DocumentId.ToString("N"),
                Note = d.Document != null ? (d.Document.Description ?? d.Document.Id.ToString("N")) : d.DocumentId.ToString("N") // ĐÃ SỬA: dùng Description thay vì Note (nếu có cột Note thì thay lại)
            })
            .ToListAsync();

        var groupedDocs = docs.GroupBy(x => x.ProductId)
                              .ToDictionary(g => g.Key, g => g.ToList());

        var result = new List<InventoryDetailReportItemModel>();

        foreach (var p in pagedProducts)
        {
            var pid = p.ProductId; // Không dùng .Value
            var trans = groupedDocs.TryGetValue(pid, out var list) ? list.Cast<dynamic>() : Enumerable.Empty<dynamic>();
            var opening = openingDict.GetValueOrDefault(pid, 0m);
            var closing = closingDict.GetValueOrDefault(pid, 0m);

            // === NHẬP ===
            var nhapHang = trans.Where(x => x.TransType == TransType.GoodsReceipt).Sum(x => x.Quantity);
            var xuatTraLai = trans.Where(x => x.TransType == TransType.ReturnOutput).Sum(x => x.Quantity);
            var nhapChuyen = trans.Where(x => x.TransType == TransType.Exportrotation && (x.ToStoreId == storeId || x.StoreId == storeId)).Sum(x => x.Quantity);

            // === XUẤT ===
            var xuatHoan = trans.Where(x => x.TransType == TransType.ReturnedGoodsReceipt).Sum(x => x.Quantity);
            var huySP = trans.Where(x => x.TransType == TransType.CanceledProduct).Sum(x => x.Quantity);
            var xuatChuyen = trans.Where(x => x.TransType == TransType.Exportrotation && x.StoreId == storeId).Sum(x => x.Quantity);

            var totalInput = nhapHang + xuatTraLai + nhapChuyen;
            var totalOutput = xuatHoan + huySP + xuatChuyen;
            var sold = opening + totalInput - totalOutput - closing;

            // === ĐIỀU CHỈNH (có chi tiết) ===
            // Nếu dự án bạn KHÔNG có TransType.InventoryAdjustment → dùng TransType == TransType.Adjustment hoặc bỏ điều kiện
            var adjustments = trans.Where(x => x.TransType.ToString().Contains("Adjustment") || x.TransType.ToString().Contains("Adjust"))
                .Select(x => new InventoryAdjustmentDetailModel
                {
                    Quantity = x.Quantity,
                    Date = x.CreateDate,
                    DocumentCode = x.DocumentCode,
                    Note = x.Note
                }).ToList();

            result.Add(new InventoryDetailReportItemModel
            {
                SKU = p.SKU,
                Name = p.Name,
                Unit = p.Unit.ToString(),
                OpeningQuantity = opening,
                ClosingQuantity = closing,
                SoldQuantity = sold >= 0 ? sold : 0,
                TotalInput = totalInput,
                TotalOutput = totalOutput,
                InputBreakdown = new InventoryInputBreakdownModel
                {
                    GoodsReceipt = nhapHang,
                    ReturnOutput = xuatTraLai,
                    ImportRotation = nhapChuyen
                },
                OutputBreakdown = new InventoryOutputBreakdownModel
                {
                    ReturnedGoodsReceipt = xuatHoan,
                    CanceledProduct = huySP,
                    ExportRotation = xuatChuyen
                },
                Adjustments = adjustments
            });
        }

        return new InventoryDetailReportModel
        {
            Products = new QueryResult<InventoryDetailReportItemModel>
            {
                Data = result,
                TotalCount = totalCount
            }
        };
    }

    public async Task<SalesDetailReportModel> GetSalesDetailReportAsync(QueryInfo queryInfo, GetSalesDetailReportQuery query)
    {
        var fromDate = query.FromDate.Date;
        var toDate = query.ToDate.Date.AddDays(1);

        // 1. Lấy đơn hàng (chỉ ShippingType = None + Confirmed)
        var ordersQuery = _context.SalesOrders
            .AsNoTracking()
            .Where(o => o.OrderStatus == OrderStatusType.Confirmed
                        //&& o.ShippingType == ShippingType.None
                        && o.ModifiedDate >= fromDate
                        && o.ModifiedDate < toDate);

        if (query.StoreId.HasValue)
            ordersQuery = ordersQuery.Where(o => o.StoreId == query.StoreId.Value);



        // Lọc theo ShippingType nếu có truyền vào
        if (query.ShippingType.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.ShippingType == query.ShippingType.Value);
        }

        // 2. Join với Item + Product + Promotion (dùng SalesOrderItemId để lấy đúng giảm giá từng món)
        var rawDataQuery = from order in ordersQuery
                           join item in _context.SalesOrderItems.AsNoTracking()
                               on order.Id equals item.SalesOrderId
                           join product in _context.Products.AsNoTracking()
                               on item.ProductId equals product.Id
                           // Left join để lấy giảm giá chính xác từ SalesOrderPromotion (nếu có)
                           join promo in _context.SalesOrderPromotions.AsNoTracking()
                               on item.Id equals promo.SalesOrderItemId into promos
                           from promo in promos.DefaultIfEmpty()
                           where !query.ProductGroupId.HasValue || product.ProductGroupId == query.ProductGroupId.Value  // THÊM DÒNG NÀY
                           select new
                           {
                               order.ModifiedDate,
                               order.OrderCode,
                               product.SKU,
                               product.Name,
                               item.Quantity,
                               item.TotalPrice,                    // Giá bán thực tế (sau giảm tay nếu có)
                               DiscountAmount = promo != null ? promo.DiscountAmount : 0m
                           };

        // 3. Tìm kiếm (nếu có)
        if (!string.IsNullOrEmpty(queryInfo.SearchText))
        {
            var keyword = $"%{queryInfo.SearchText}%";
            rawDataQuery = rawDataQuery.Where(x =>
                EF.Functions.Collate(x.SKU, "Latin1_General_CI_AI").Contains(keyword) ||
                EF.Functions.Collate(x.Name, "Latin1_General_CI_AI").Contains(keyword));
        }

        // 4. Gom nhóm theo từng dòng item (vì 1 item có thể có nhiều promotion → sum lại)
        var groupedData = await rawDataQuery
            .GroupBy(x => new
            {
                x.ModifiedDate,
                x.OrderCode,
                x.SKU,
                x.Name,
                x.Quantity,
                x.TotalPrice
            })
            .Select(g => new
            {
                g.Key.ModifiedDate,
                g.Key.OrderCode,
                g.Key.SKU,
                g.Key.Name,
                g.Key.Quantity,
                g.Key.TotalPrice,
                TotalDiscount = g.Sum(x => x.DiscountAmount)  // ← CHÍNH XÁC 100% TỪNG SẢN PHẨM
            })
            .ToListAsync();

        // 5. Tạo danh sách chi tiết
        var itemDetails = groupedData.Select(x => new SalesDetailReportItemModel
        {
            DateTime = x.ModifiedDate,
            OrderCode = x.OrderCode,
            SKU = x.SKU,
            ProductName = x.Name,
            Quantity = x.Quantity,
            BeforeDiscount = x.TotalPrice + x.TotalDiscount,   // Tiền trước khi giảm (giá gốc hoặc giá đã giảm tay)
            Discount = x.TotalDiscount,                        // ← ĐÚNG: 0, 1500, 2000...
            Commission = null,
            ServiceFee = null,
            Tax = null,
            Revenue = x.TotalPrice                             // Tiền thực nhận (PaidAmount của item)
        }).ToList();

        // 6. Sắp xếp (theo QueryInfo)
        itemDetails = queryInfo.OrderBy switch
        {
            "DateTime" => queryInfo.OrderType == OrderType.Ascending
                ? itemDetails.OrderBy(x => x.DateTime).ToList()
                : itemDetails.OrderByDescending(x => x.DateTime).ToList(),
            "OrderCode" => queryInfo.OrderType == OrderType.Ascending
                ? itemDetails.OrderBy(x => x.OrderCode).ToList()
                : itemDetails.OrderByDescending(x => x.OrderCode).ToList(),
            "SKU" => queryInfo.OrderType == OrderType.Ascending
                ? itemDetails.OrderBy(x => x.SKU).ToList()
                : itemDetails.OrderByDescending(x => x.SKU).ToList(),
            "ProductName" => queryInfo.OrderType == OrderType.Ascending
                ? itemDetails.OrderBy(x => x.ProductName).ToList()
                : itemDetails.OrderByDescending(x => x.ProductName).ToList(),
            "Quantity" => queryInfo.OrderType == OrderType.Ascending
                ? itemDetails.OrderBy(x => x.Quantity).ToList()
                : itemDetails.OrderByDescending(x => x.Quantity).ToList(),
            "BeforeDiscount" => queryInfo.OrderType == OrderType.Ascending
                ? itemDetails.OrderBy(x => x.BeforeDiscount).ToList()
                : itemDetails.OrderByDescending(x => x.BeforeDiscount).ToList(),
            "Discount" => queryInfo.OrderType == OrderType.Ascending
                ? itemDetails.OrderBy(x => x.Discount).ToList()
                : itemDetails.OrderByDescending(x => x.Discount).ToList(),
            "Revenue" => queryInfo.OrderType == OrderType.Ascending
                ? itemDetails.OrderBy(x => x.Revenue).ToList()
                : itemDetails.OrderByDescending(x => x.Revenue).ToList(),
            _ => itemDetails.OrderByDescending(x => x.DateTime).ToList()
        };

        // 7. Summary
        var summary = new SalesDetailReportSummaryModel
        {
            TotalQuantity = itemDetails.Sum(x => x.Quantity),
            TotalBeforeDiscount = itemDetails.Sum(x => x.BeforeDiscount),
            TotalAfterDiscount = itemDetails.Sum(x => x.Revenue)
        };

        // 8. Phân trang
        var totalCount = queryInfo.NeedTotalCount ? itemDetails.Count : 0;
        var paged = itemDetails.Skip(queryInfo.Skip).Take(queryInfo.Top).ToList();

        return new SalesDetailReportModel
        {
            Summary = summary,
            Details = new QueryResult<SalesDetailReportItemModel>
            {
                Data = paged,
                TotalCount = totalCount
            }
        };
    }

    //public async Task<DailySalesReportModel> GetDailySalesReportAsync(
    //    QueryInfo queryInfo,
    //    GetDailySalesReportQuery query)
    //{
    //    var fromDate = query.FromDate.Date;
    //    var toDateEnd = query.ToDate.Date.AddDays(1);

    //    // ===================================================================
    //    // 1. Lấy dữ liệu thô: SalesOrder + Item + Product + Promotion chi tiết
    //    // ===================================================================
    //    var rawData = await (
    //        from order in _context.SalesOrders.AsNoTracking()
    //        join item in _context.SalesOrderItems.AsNoTracking() on order.Id equals item.SalesOrderId
    //        join product in _context.Products.AsNoTracking() on item.ProductId equals product.Id
    //        join pg in _context.ProductGroups.AsNoTracking() on product.ProductGroupId equals pg.Id into pgGroup
    //        from productGroup in pgGroup.DefaultIfEmpty()

    //            // === Lấy FoodAppPrice theo ShippingType ===
    //        let foodAppPrice = (
    //            from fap in _context.FoodAppPrices.AsNoTracking()
    //            join platform in _context.FoodAppPlatforms.AsNoTracking()
    //                on fap.FoodAppPlatformId equals platform.Id
    //            where fap.ProductId == product.Id
    //                  && platform.ShippingType == order.ShippingType
    //                  && fap.IsActive && platform.IsActive
    //            select fap.FoodAppPrice
    //        ).FirstOrDefault()

    //        // === Lấy giảm giá CHI TIẾT theo từng item từ SalesOrderPromotion ===
    //        let itemDiscount = (
    //            from promo in _context.SalesOrderPromotions.AsNoTracking()
    //            where promo.SalesOrderItemId == item.Id
    //            select promo.DiscountAmount
    //        ).Sum()

    //        where order.StoreId == query.StoreId
    //              && order.OrderStatus == OrderStatusType.Confirmed
    //              && order.ModifiedDate >= fromDate
    //              && order.ModifiedDate < toDateEnd

    //        select new
    //        {
    //            OrderCode = order.OrderCode,
    //            ShippingType = order.ShippingType,
    //            PaymentMethod = order.PaymentMethod ?? PaymentMethodType.COD,

    //            ProductId = product.Id,
    //            ProductSku = product.SKU,
    //            ProductName = product.Name,
    //            ProductGroupName = productGroup != null ? productGroup.Name : "Không nhóm",

    //            Quantity = item.Quantity,
    //            //UnitPrice = order.ShippingType == ShippingType.None
    //            //    ? product.Price
    //            //    : (foodAppPrice > 0 ? foodAppPrice : product.Price),

    //            // Giảm giá thực tế của item này (có thể = 0)
    //            UnitPrice = item.UnitPrice + (itemDiscount / item.Quantity),
    //            DiscountAmount = itemDiscount,

    //            // Doanh thu trước giảm giá
    //            //OriginalAmount = item.Quantity * (
    //            //    order.ShippingType == ShippingType.None
    //            //        ? product.Price
    //            //        : (foodAppPrice > 0 ? foodAppPrice : product.Price)
    //            //)
    //            OriginalAmount = item.Quantity * (item.UnitPrice + (itemDiscount / item.Quantity)),
    //            OrderId = order.Id
    //        }
    //    ).ToListAsync();

    //    if (!rawData.Any())
    //    {
    //        return new DailySalesReportModel
    //        {
    //            FromDate = query.FromDate,
    //            ToDate = query.ToDate,
    //            StoreId = query.StoreId,
    //            Summary = new(),
    //            Details = new()
    //        };
    //    }

    //    // ===================================================================
    //    // 2. Tính toán NetRevenue = OriginalAmount - DiscountAmount
    //    // ===================================================================
    //    var processed = rawData.Select(x => new
    //    {
    //        x.ProductId,
    //        x.ProductSku,
    //        x.ProductName,
    //        x.ProductGroupName,
    //        x.ShippingType,
    //        x.PaymentMethod,
    //        x.Quantity,
    //        x.UnitPrice,
    //        x.OriginalAmount,
    //        DiscountAmount = x.DiscountAmount, // lấy trực tiếp từ bảng Promotion
    //        NetRevenue = x.OriginalAmount - x.DiscountAmount,
    //        x.OrderCode
    //    }).ToList();

    //    // ===================================================================
    //    // 3. Gom nhóm theo Product → ShippingType + PaymentMethod
    //    // ===================================================================
    //    var details = processed
    //        .GroupBy(x => new { x.ProductId, x.ProductSku, x.ProductName, x.ProductGroupName })
    //        .Select(g => new DailySalesDetailModel
    //        {
    //            ProductId = g.Key.ProductId,
    //            ProductSku = g.Key.ProductSku,
    //            ProductName = g.Key.ProductName,
    //            ProductGroupName = g.Key.ProductGroupName,

    //            Items = g
    //                .GroupBy(i => new { i.ShippingType, i.PaymentMethod })
    //                .Select(ig => new DailySalesItemModel
    //                {
    //                    ShippingType = ig.Key.ShippingType,
    //                    PaymentMethod = ig.Key.PaymentMethod,
    //                    Quantity = ig.Sum(x => x.Quantity),
    //                    UnitPrice = ig.First().UnitPrice,
    //                    OriginalAmount = ig.Sum(x => x.OriginalAmount),
    //                    DiscountAmount = ig.Sum(x => x.DiscountAmount),
    //                    NetRevenue = ig.Sum(x => x.NetRevenue),
    //                    OrderCount = ig.Select(x => x.OrderCode).Distinct().Count()
    //                })
    //                .OrderBy(x => GetShippingTypeOrder(x.ShippingType))
    //                .ThenBy(x => GetPaymentMethodOrder(x.PaymentMethod))
    //                .ToList(),

    //            TotalQuantity = g.Sum(x => x.Quantity),
    //            TotalOriginalAmount = g.Sum(x => x.OriginalAmount),
    //            TotalDiscountAmount = g.Sum(x => x.DiscountAmount),
    //            TotalNetRevenue = g.Sum(x => x.NetRevenue)
    //        })
    //        .OrderBy(x => x.ProductName)
    //        .ToList();

    //    // ===================================================================
    //    // 4. Summary + Paging
    //    // ===================================================================
    //    var summary = new DailySalesSummaryModel
    //    {
    //        TotalQuantitySold = details.Sum(x => x.TotalQuantity),
    //        TotalOriginalAmount = details.Sum(x => x.TotalOriginalAmount),
    //        TotalDiscountAmount = details.Sum(x => x.TotalDiscountAmount),
    //        TotalNetRevenue = details.Sum(x => x.TotalNetRevenue)
    //    };

    //    var pagedDetails = details
    //        .Skip(queryInfo.Skip)
    //        .Take(queryInfo.Top)
    //        .ToList();

    //    return new DailySalesReportModel
    //    {
    //        FromDate = query.FromDate,
    //        ToDate = query.ToDate,
    //        StoreId = query.StoreId,
    //        Summary = summary,
    //        Details = pagedDetails
    //    };
    //}

    //// ===================================================================
    //// Helper sort – đúng thứ tự ưu tiên thực tế của bạn
    //// ===================================================================
    //private static int GetShippingTypeOrder(ShippingType type) => type switch
    //{
    //    ShippingType.None => 1,
    //    ShippingType.GrabFood => 2,
    //    ShippingType.ShopeeFood => 3,
    //    ShippingType.XanhSMart => 4,
    //    ShippingType.LoShip => 5,
    //    ShippingType.BeFood => 6,
    //    ShippingType.Ahamove => 7,
    //    ShippingType.GoFood => 8,
    //    ShippingType.Foody => 9,
    //    ShippingType.Lozi => 10,
    //    ShippingType.DeliveryK => 11,
    //    ShippingType.Capichi => 12,
    //    _ => 999
    //};

    //private static int GetPaymentMethodOrder(PaymentMethodType method) => method switch
    //{
    //    PaymentMethodType.COD => 1,
    //    PaymentMethodType.Card => 2,
    //    PaymentMethodType.Online => 3,
    //    PaymentMethodType.Wallet => 4,
    //    PaymentMethodType.BankTransfer => 5,
    //    PaymentMethodType.GiftCard => 6,
    //    PaymentMethodType.Other => 7,
    //    _ => 999
    //};

    public async Task<DailySalesReportModel> GetDailySalesReportAsync(GetDailySalesReportQuery query)
    {
        var fromDate = query.FromDate.Date;
        var toDateEnd = query.ToDate.Date.AddDays(1);

        var orders = _context.SalesOrders
            .AsNoTracking()
            .Where(o => o.StoreId == query.StoreId
                         && o.OrderStatus == OrderStatusType.Confirmed
                         && o.ModifiedDate >= fromDate
                         && o.ModifiedDate < toDateEnd);

        if (query.ShippingType.HasValue)
            orders = orders.Where(o => o.ShippingType == query.ShippingType.Value);

        if (query.PaymentMethod.HasValue)
            orders = orders.Where(o => o.PaymentMethod == query.PaymentMethod.Value);

        var itemsQuery = orders.SelectMany(o => o.SalesOrderItem!);

        if (query.ProductGroupId.HasValue)
        {
            itemsQuery = itemsQuery.Where(i => i.Product!.ProductGroupId == query.ProductGroupId.Value);
        }

        if (query.ProductId.HasValue)
        {
            itemsQuery = itemsQuery.Where(i => i.ProductId == query.ProductId.Value);
        }

        // Lấy thông tin filter để trả về
        var filterInfo = (query.ProductGroupId.HasValue || query.ProductId.HasValue)
            ? await itemsQuery
                .Select(i => new
                {
                    ProductGroupName = i.Product!.ProductGroup!.Name,
                    ProductName = i.Product!.Name,
                    SKU = i.Product!.SKU
                })
                .FirstOrDefaultAsync()
            : null;

        var shippingTypeName = query.ShippingType switch
        {
            ShippingType.None => "Tại quán",
            ShippingType.GrabFood => "GrabFood",
            ShippingType.ShopeeFood => "ShopeeFood",
            ShippingType.XanhSMart => "Xanh SMart",
            ShippingType.LoShip => "LoShip",
            ShippingType.BeFood => "BeFood",
            ShippingType.Ahamove => "Ahamove",
            ShippingType.GoFood => "GoFood",
            ShippingType.Foody => "Foody",
            ShippingType.Lozi => "Lozi",
            ShippingType.DeliveryK => "DeliveryK",
            ShippingType.Capichi => "Capichi",
            _ => query.ShippingType?.ToString() ?? null
        };

        var paymentMethodName = query.PaymentMethod switch
        {
            PaymentMethodType.COD => "Tiền mặt",
            PaymentMethodType.Card => "Thẻ",
            PaymentMethodType.Online => "Thanh toán online",
            PaymentMethodType.Wallet => "Ví điện tử",
            PaymentMethodType.BankTransfer => "Chuyển khoản",
            PaymentMethodType.GiftCard => "Thẻ quà tặng",
            PaymentMethodType.Other => "Khác",
            _ => query.PaymentMethod?.ToString() ?? null
        };

        // Tính toán tổng hợp
        // Sửa lỗi aggregate lồng nhau – đưa về client-side tính
        var rawStats = await itemsQuery
            .Select(item => new
            {
                item.Quantity,
                item.UnitPrice,
                ItemDiscount = item.SalesOrder!.SalesOrderPromotion!
                    .Where(p => p.SalesOrderItemId == item.Id)
                    .Sum(p => (decimal?)p.DiscountAmount) ?? 0m,
                OrderLevelDiscount = item.SalesOrder!.SalesOrderPromotion!
                    .Where(p => p.SalesOrderItemId == null)
                    .Sum(p => (decimal?)p.DiscountAmount) ?? 0m,
                item.TotalPrice,
                item.SalesOrderId
            })
            .ToListAsync();

        var stats = rawStats.Any()
            ? new
            {
                TotalOrders = rawStats.Select(x => x.SalesOrderId).Distinct().Count(),
                TotalQuantitySold = rawStats.Sum(x => x.Quantity),
                TotalOriginalAmount = rawStats.Sum(x => x.Quantity * (x.UnitPrice + (x.Quantity > 0 ? x.ItemDiscount / x.Quantity : 0m))),
                TotalDiscountAmount = rawStats.Sum(x => x.ItemDiscount + x.OrderLevelDiscount),
                TotalNetRevenue = rawStats.Sum(x => x.TotalPrice)
            }
            : new { TotalOrders = 0, TotalQuantitySold = 0, TotalOriginalAmount = 0m, TotalDiscountAmount = 0m, TotalNetRevenue = 0m };




        return new DailySalesReportModel
        {
            FromDate = query.FromDate,
            ToDate = query.ToDate,
            StoreId = query.StoreId,
            StoreName = await _context.Stores
                .AsNoTracking()
                .Where(s => s.Id == query.StoreId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync() ?? "Cửa hàng không xác định",

            ProductGroupName = filterInfo?.ProductGroupName,
            ProductName = filterInfo?.ProductName,
            SKU = filterInfo?.SKU,
            ShippingType = shippingTypeName,
            PaymentMethod = paymentMethodName,

            TotalOrders = stats?.TotalOrders ?? 0,
            TotalQuantitySold = stats?.TotalQuantitySold ?? 0,
            TotalOriginalAmount = Math.Round(stats?.TotalOriginalAmount ?? 0, 2),
            TotalDiscountAmount = Math.Round(stats?.TotalDiscountAmount ?? 0, 2),
            TotalNetRevenue = Math.Round(stats?.TotalNetRevenue ?? 0, 2)
        };
    }
}