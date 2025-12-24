namespace Infrastructure;

[RegisterService(typeof(IProductsRepository))]
public class ProductsRepository(
    EfApplicationDbContext context) : RepositoryBase<ProductEntity>(context), IProductsRepository
{
    private readonly EfApplicationDbContext _context = context;

    public async Task<QueryResult<Product>> GetAllWithPagingAsync(QueryInfo queryInfo)
    {
        var query = from product in _context.Products.AsNoTracking()
                    where product.IsActive == queryInfo.IsActive
                          && (EF.Functions.Collate(product.Name, "Latin1_General_CI_AI")
                              .Contains(queryInfo.SearchText ?? "") ||
                           EF.Functions.Collate(product.SKU, "Latin1_General_CI_AI")
                              .Contains(queryInfo.SearchText ?? ""))
                    orderby product.CreateDate descending
                    select new Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Quantity = _context.Inventories.Where(i => i.ProductId == product.Id).Sum(i => (int?)i.Quantity) ?? 0,
                        ProductGroup = new ProductGroupBasicModel
                        {
                            Id = product.ProductGroup!.Id,
                            Name = product.ProductGroup!.Name,
                        },
                        Price = product.Price,
                        Cost = product.Cost,
                        SKU = product.SKU,
                        Picture = product.Picture,
                        IsActive = product.IsActive,
                        Unit = product.Unit.ToString(),
                        VAT = product.VAT,
                        Description = product.Description,
                        CreateDate = product.CreateDate
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
        return new QueryResult<Product>
        {
            Data = pagedEntities,
            TotalCount = totalCount
        };
    }

    //public async Task<QueryResult<ProductWithPromotion>> GetProductsByStoreIdWithPaging(Guid storeId, QueryInfo queryInfo)
    //{
    //    var now = DateTime.Now; // hoặc DateTime.UtcNow 

    //    var query = _context.Products
    //        .AsNoTracking()
    //        .Join(_context.Inventories.AsNoTracking(),
    //            p => p.Id,
    //            i => i.ProductId,
    //            (p, i) => new { product = p, inventory = i })
    //        .Where(x => x.inventory.StoreId == storeId &&
    //                    EF.Functions.Collate(x.product.Name, "Latin1_General_CI_AI")
    //                        .Contains(queryInfo.SearchText ?? "") &&
    //                    x.product.IsActive == queryInfo.IsActive);

    //    int totalCount = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;

    //    var pagedEntities = await query
    //        .OrderByDescending(x => x.inventory.Quantity)
    //        .Skip(queryInfo.Skip)
    //        .Take(queryInfo.Top)
    //        .Select(x => new ProductWithPromotion
    //        {
    //            Id = x.product.Id,
    //            Name = x.product.Name,
    //            Quantity = x.inventory.Quantity,
    //            ProductGroup = new ProductGroupBasicModel
    //            {
    //                Id = x.product.ProductGroup!.Id,
    //                Name = x.product.ProductGroup!.Name
    //            },
    //            Cost = x.product.Cost,
    //            Price = x.product.Price,
    //            SalePrice = x.product.Price, // sẽ được cập nhật sau
    //            AppliedPromotionCode = null,

    //            SKU = x.product.SKU,
    //            Picture = x.product.Picture,
    //            Description = x.product.Description,
    //            Unit = x.product.Unit.ToString(),
    //            IsActive = x.product.IsActive,
    //            VAT = x.product.VAT,
    //            CreateDate = x.product.CreateDate
    //        })
    //        .ToListAsync();

    //    // Tính SalePrice + mã khuyến mãi 
    //    foreach (var item in pagedEntities)
    //    {
    //        var promotion = _context.PromotionProducts
    //            .Where(pp => pp.ProductId == item.Id &&
    //                         pp.Promotion.IsActive &&
    //                         pp.Promotion.StartDate <= now &&
    //                         pp.Promotion.EndDate >= now)
    //            .Select(pp => new {
    //                Id = pp.Promotion.Id,          // SỬA TẠI ĐÂY: thêm Id
    //                Code = pp.Promotion.Code,
    //                pp.Promotion.Value,
    //                pp.Promotion.Type
    //            })
    //            .FirstOrDefault()

    //            ?? _context.PromotionProductGroups
    //                .Where(ppg => ppg.ProductGroupId == item.ProductGroup!.Id &&
    //                              ppg.Promotion.IsActive &&
    //                              ppg.Promotion.StartDate <= now &&
    //                              ppg.Promotion.EndDate >= now)
    //                .Select(ppg => new {
    //                    Id = ppg.Promotion.Id,     // SỬA TẠI ĐÂY: thêm Id
    //                    Code = ppg.Promotion.Code,
    //                    ppg.Promotion.Value,
    //                    ppg.Promotion.Type
    //                })
    //                .FirstOrDefault()

    //            ?? _context.PromotionStores
    //                .Where(ps => ps.StoreId == storeId &&
    //                             ps.Promotion.IsActive &&
    //                             ps.Promotion.StartDate <= now &&
    //                             ps.Promotion.EndDate >= now)
    //                .Select(ps => new {
    //                    Id = ps.Promotion.Id,      // SỬA TẠI ĐÂY: thêm Id
    //                    Code = ps.Promotion.Code,
    //                    ps.Promotion.Value,
    //                    ps.Promotion.Type
    //                })
    //                .FirstOrDefault()

    //            ?? _context.Promotions
    //                .Where(p => p.StoreScope == 0 &&
    //                            p.ProductScope == 0 &&
    //                            p.IsActive &&
    //                            p.StartDate <= now &&
    //                            p.EndDate >= now)
    //                .Select(p => new {
    //                    Id = p.Id,                 // SỬA TẠI ĐÂY: thêm Id
    //                    Code = p.Code,
    //                    p.Value,
    //                    p.Type
    //                })
    //                .FirstOrDefault();

    //        if (promotion != null)
    //        {
    //            item.AppliedPromotionId = promotion.Id;        // ← Gán PromotionId
    //            item.AppliedPromotionCode = promotion.Code;   // ← Vẫn giữ Code nếu muốn hiển thị
    //            item.SalePrice = promotion.Type == PromotionType.Percentage
    //                ? item.Price * (1 - promotion.Value / 100m)
    //                : item.Price - promotion.Value;
    //        }
    //        else
    //        {
    //            item.AppliedPromotionId = null;
    //            item.AppliedPromotionCode = null;
    //            item.SalePrice = item.Price;
    //        }
    //    }

    //    return new QueryResult<ProductWithPromotion>
    //    {
    //        Data = pagedEntities,
    //        TotalCount = totalCount
    //    };
    //}

    public async Task<QueryResult<ProductWithPromotion>> GetProductsByStoreIdWithPaging(
    Guid storeId,
    QueryInfo queryInfo)
    {
        var now = DateTime.UtcNow; // Luôn dùng UTC cho promotion

        // BƯỚC 1: Lấy danh sách sản phẩm có phân trang (chỉ 1 query)
        var productQuery = _context.Products
            .AsNoTracking()
            .Join(_context.Inventories.AsNoTracking(),
                p => p.Id,
                i => i.ProductId,
                (p, i) => new { product = p, inventory = i })
            .Where(x => x.inventory.StoreId == storeId &&
                        (string.IsNullOrEmpty(queryInfo.SearchText) ||
                         EF.Functions.Collate(x.product.Name, "Latin1_General_CI_AI").Contains(queryInfo.SearchText)) &&
                        x.product.IsActive == queryInfo.IsActive);

        // Đếm tổng (nếu cần)
        int totalCount = queryInfo.NeedTotalCount
            ? await productQuery.CountAsync()
            : 0;

        // Lấy dữ liệu phân trang + các trường cần thiết (vẫn chỉ 1 query)
        var pagedProducts = await productQuery
            .OrderByDescending(x => x.inventory.Quantity)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(x => new
            {
                x.product.Id,
                x.product.Name,
                x.inventory.Quantity,
                ProductGroupId = (Guid?)x.product.ProductGroupId, // có thể null
                ProductGroupName = x.product.ProductGroup != null ? x.product.ProductGroup.Name : null,
                x.product.Cost,
                x.product.Price,
                x.product.SKU,
                x.product.Picture,
                x.product.Description,
                x.product.Unit,
                x.product.IsActive,
                x.product.VAT,
                x.product.CreateDate
            })
            .ToListAsync();

        // Nếu không có sản phẩm → trả về luôn
        if (!pagedProducts.Any())
        {
            return new QueryResult<ProductWithPromotion>
            {
                Data = new List<ProductWithPromotion>(),
                TotalCount = totalCount
            };
        }

        // Thu thập tất cả ID cần kiểm tra khuyến mãi
        var productIds = pagedProducts.Select(p => p.Id).ToList();
        var groupIds = pagedProducts
            .Where(p => p.ProductGroupId.HasValue)
            .Select(p => p.ProductGroupId.GetValueOrDefault())
            .Distinct()
            .ToList();

        // BƯỚC 2: LẤY TẤT CẢ KHUYẾN MÃI ĐANG CÓ HIỆU LỰC CHỈ TRONG 1 QUERY DUY NHẤT
        var activePromotions = await _context.Promotions
            .AsNoTracking()
            .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now && (p.UsageLimit == null || p.UsageCount < p.UsageLimit))
            .Select(p => new
            {
                p.Id,
                p.Code,
                p.Value,
                p.Type,
                p.StoreScope,    // 0 = tất cả cửa hàng
                p.ProductScope,  // 0 = tất cả sản phẩm
                                 // Lấy danh sách áp dụng chi tiết
                AppliedProductIds = p.PromotionProducts.Select(pp => pp.ProductId),
                AppliedGroupIds = p.PromotionProductGroups.Select(ppg => ppg.ProductGroupId),
                AppliedStoreIds = p.PromotionStores.Select(ps => ps.StoreId)
            })
            .ToListAsync();

        // BƯỚC 3: Tính khuyến mãi tốt nhất cho từng sản phẩm (trong RAM - cực nhanh)
        var result = new List<ProductWithPromotion>();

        foreach (var item in pagedProducts)
        {
            // Tìm khuyến mãi tốt nhất theo thứ tự ưu tiên: Sản phẩm > Nhóm > Cửa hàng > Toàn hệ thống
            var bestPromo = activePromotions
                .Where(p =>
                    p.AppliedProductIds.Contains(item.Id) || // Ưu tiên cao nhất
                    (item.ProductGroupId.HasValue && p.AppliedGroupIds.Contains(item.ProductGroupId.Value)) ||
                    p.AppliedStoreIds.Contains(storeId) ||
                    (p.StoreScope == 0 && p.ProductScope == 0))
                .OrderByDescending(p =>
                    p.AppliedProductIds.Contains(item.Id) ? 4 :
                    (item.ProductGroupId.HasValue && p.AppliedGroupIds.Contains(item.ProductGroupId.Value)) ? 3 :
                    p.AppliedStoreIds.Contains(storeId) ? 2 : 1)
                .ThenByDescending(p => p.Value) // cùng cấp thì chọn giảm nhiều nhất
                .FirstOrDefault();

            var salePrice = item.Price;
            Guid? promotionId = null;
            string? promotionCode = null;

            if (bestPromo != null)
            {
                promotionId = bestPromo.Id;
                promotionCode = bestPromo.Code;

                salePrice = bestPromo.Type == PromotionType.Percentage
                    ? item.Price * (1 - bestPromo.Value / 100m)
                    : item.Price - bestPromo.Value;
            }

            result.Add(new ProductWithPromotion
            {
                Id = item.Id,
                Name = item.Name,
                Quantity = item.Quantity,
                ProductGroup = new ProductGroupBasicModel
                {
                    Id = item.ProductGroupId ?? Guid.Empty,
                    Name = item.ProductGroupName ?? "Không có nhóm"
                },
                Cost = item.Cost,
                Price = item.Price,
                SalePrice = salePrice,
                AppliedPromotionId = promotionId,
                AppliedPromotionCode = promotionCode,
                SKU = item.SKU,
                Picture = item.Picture,
                Description = item.Description,
                Unit = item.Unit.ToString(),
                IsActive = item.IsActive,
                VAT = item.VAT,
                CreateDate = item.CreateDate
            });
        }

        return new QueryResult<ProductWithPromotion>
        {
            Data = result,
            TotalCount = totalCount
        };
    }

    //public async Task<QueryResult<ProductWithPromotion>> GetProductsByStoreIdWithPaging(Guid storeId, QueryInfo queryInfo)
    //{
    //    var now = DateTime.Now; // Đảm bảo có ngày để check

    //    var query = _context.Products
    //        .AsNoTracking()
    //        .Join(
    //            _context.Inventories.AsNoTracking(),
    //            product => product.Id,
    //            inventory => inventory.ProductId,
    //            (product, inventory) => new { product, inventory }
    //        )
    //        .Where(x =>
    //            x.inventory.StoreId == storeId &&
    //            EF.Functions.Collate(x.product.Name, "Latin1_General_CI_AI")
    //                .Contains(queryInfo.SearchText ?? "") &&
    //            x.product.IsActive == queryInfo.IsActive
    //        );

    //    int totalCount = 0;
    //    if (queryInfo.NeedTotalCount)
    //    {
    //        totalCount = await query.CountAsync();
    //    }

    //    var pagedEntities = await query
    //        //.OrderByDescending(x => x.product.CreateDate)
    //        .OrderByDescending(x => x.inventory.Quantity)
    //        .Skip(queryInfo.Skip)
    //        .Take(queryInfo.Top)
    //        .Select(x => new ProductWithPromotion
    //        {
    //            Id = x.product.Id,
    //            Name = x.product.Name,
    //            Quantity = x.inventory.Quantity,
    //            ProductGroup = new ProductGroupBasicModel
    //            {
    //                Id = x.product.ProductGroup!.Id,
    //                Name = x.product.ProductGroup!.Name
    //            },
    //            Cost = x.product.Cost,
    //            Price = x.product.Price,
    //            AppliedPromotionCode = null as string,
    //            SalePrice =
    //            (
    //                // Ưu tiên 1: promotion theo sản phẩm
    //                _context.PromotionProducts
    //                    .Where(pp => pp.ProductId == x.product.Id &&
    //                        pp.Promotion.IsActive )
    //                        // && pp.Promotion.StartDate <= now && pp.Promotion.EndDate >= now)
    //                    .Select(pp => x.product.Price -
    //                        (pp.Promotion.Type == PromotionType.Percentage
    //                            ? (x.product.Price * pp.Promotion.Value / 100)
    //                            : pp.Promotion.Value))
    //                            .Cast<decimal?>()
    //                    .FirstOrDefault()

    //                // Nếu null thì sang nhóm sản phẩm
    //                ??
    //                _context.PromotionProductGroups
    //                    .Where(ppg => ppg.ProductGroupId == x.product.ProductGroupId &&
    //                        ppg.Promotion.IsActive )
    //                       // &&  ppg.Promotion.StartDate <= now && ppg.Promotion.EndDate >= now)
    //                    .Select(ppg => x.product.Price -
    //                        (ppg.Promotion.Type == PromotionType.Percentage
    //                            ? (x.product.Price * ppg.Promotion.Value / 100)
    //                            : ppg.Promotion.Value))
    //                            .Cast<decimal?>()
    //                    .FirstOrDefault()

    //                // Nếu null tiếp thì sang group
    //                ??
    //                _context.PromotionStores
    //                    .Where(ps => ps.StoreId == storeId && 
    //                        ps.Promotion.IsActive )
    //                       // &&  ps.Promotion.StartDate <= now && ps.Promotion.EndDate >= now)
    //                    .Select(ps => x.product.Price -
    //                        (ps.Promotion.Type == PromotionType.Percentage
    //                            ? (x.product.Price * ps.Promotion.Value / 100)
    //                            : ps.Promotion.Value))
    //                            .Cast<decimal?>()
    //                    .FirstOrDefault()
    //            ) ?? x.product.Price,

    //            SKU = x.product.SKU,
    //            Picture = x.product.Picture,
    //            Description = x.product.Description,
    //            Unit = x.product.Unit.ToString(),
    //            IsActive = x.product.IsActive,
    //            VAT = x.product.VAT,
    //            CreateDate = x.product.CreateDate
    //        })

    //        .ToListAsync();

    //    return new QueryResult<ProductWithPromotion>
    //    {
    //        Data = pagedEntities,
    //        TotalCount = totalCount
    //    };
    //}

    public async Task<QueryResult<Product>> GetProductsByCategoryGroupWithPaging(Guid storeId, string categoryGroup, QueryInfo queryInfo)
    {
        IQueryable<Product> query;

        if (storeId == Guid.Empty)
        {
            query = _context.Products
                .AsNoTracking()
                .Include(p => p.ProductGroup)
                    .ThenInclude(ppg => ppg!.ProductCategory)
                .Where(p =>
                    p.ProductGroup != null &&
                    p.ProductGroup.ProductCategory != null &&
                    p.ProductGroup.ProductCategoryId == p.ProductGroup.ProductCategory.Id &&
                    p.IsActive == queryInfo.IsActive &&
                    EF.Functions.Collate(p.Name, "Latin1_General_CI_AI").Contains(queryInfo.SearchText ?? "")
                )
                .Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name,
                    ProductGroup = new ProductGroupBasicModel
                    {
                        Id = p.ProductGroup!.Id,
                        Name = p.ProductGroup.Name
                    },
                    Cost = p.Cost,
                    SKU = p.SKU,
                    Picture = p.Picture,
                    Description = p.Description,
                    Price = p.Price,
                    Unit = p.Unit.ToString(),
                    VAT = p.VAT,
                    IsActive = p.IsActive,
                    CreateDate = p.CreateDate
                });
        }
        else
        {
            var baseQuery = _context.Products
            .AsNoTracking()
            .Include(p => p.ProductGroup)
                .ThenInclude(ppg => ppg!.ProductCategory)
            .Where(p =>
                p.ProductGroup != null &&
                p.ProductGroup.ProductCategory != null &&
                p.ProductGroup.ProductCategoryId == p.ProductGroup.ProductCategory.Id &&
                p.IsActive == queryInfo.IsActive
            )
            .Join(
                _context.Inventories.AsNoTracking(),
                product => product.Id,
                inventory => inventory.ProductId,
                (product, inventory) => new { product, inventory }
            )
            .Where(x =>
                x.inventory.StoreId == storeId &&
                EF.Functions.Collate(x.product.Name, "Latin1_General_CI_AI")
                    .Contains(queryInfo.SearchText ?? "")
            );

            query = baseQuery.Select(x => new Product
            {
                Id = x.product.Id,
                Name = x.product.Name,
                ProductGroup = new ProductGroupBasicModel
                {
                    Id = x.product.ProductGroup!.Id,
                    Name = x.product.ProductGroup.Name
                },
                Cost = x.product.Cost,
                SKU = x.product.SKU,
                Picture = x.product.Picture,
                Description = x.product.Description,
                Price = x.product.Price,
                Quantity = x.inventory.Quantity,
                Unit = x.product.Unit.ToString(),
                VAT = x.product.VAT,
                IsActive = x.product.IsActive,
                CreateDate = x.product.CreateDate
            });
        }

        int totalCount = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;

        var pagedEntities = await query
            .OrderByDescending(p => p.CreateDate)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        return new QueryResult<Product>
        {
            Data = pagedEntities,
            TotalCount = totalCount
        };
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products.AsNoTracking()
                        .GroupJoin(
                _context.Inventories.AsNoTracking(),
                product => product.Id,
                inventory => inventory.ProductId,
                (product, inventories) => new { product, inventories }
            )
            .SelectMany(
                x => x.inventories.DefaultIfEmpty(),
                (x, inventory) => new { x.product, inventory }
            )
            .Where(x => x.product.Id == id)
            .Select(x => new Product
            {
                Id = x.product.Id,
                Name = x.product.Name,
                Quantity = x.inventory != null ? x.inventory.Quantity : 0,
                ProductGroup = new ProductGroupBasicModel
                {
                    Id = x.product.ProductGroup!.Id,
                    Name = x.product.ProductGroup!.Name
                },
                Cost = x.product.Cost,
                SKU = x.product.SKU,
                Picture = x.product.Picture,
                Description = x.product.Description,
                Price = x.product.Price,
                Unit = x.product.Unit.ToString(),
                VAT = x.product.VAT,
                CreateDate = x.product.CreateDate
            })
            .FirstOrDefaultAsync();
    }
    public async Task<Product> GetBySkuAsync(string sku)
    {
        return await _context.Products
            .AsNoTracking()
            .GroupJoin(
                _context.Inventories.AsNoTracking(),
                product => product.Id,
                inventory => inventory.ProductId,
                (product, inventories) => new { product, inventories }
            )
            .SelectMany(
                x => x.inventories.DefaultIfEmpty(),
                (x, inventory) => new { x.product, inventory }
            )
            .Where(x => EF.Functions.Collate(x.product.SKU, "Latin1_General_CI_AI") == sku)
            .OrderBy(x => x.product.CreateDate)
            .Select(x => new Product
            {
                Id = x.product.Id,
                Name = x.product.Name,
                Quantity = x.inventory != null ? x.inventory.Quantity : 0,
                ProductGroup = new ProductGroupBasicModel
                {
                    Id = x.product.ProductGroup!.Id,
                    Name = x.product.ProductGroup!.Name
                },
                Cost = x.product.Cost,
                SKU = x.product.SKU,
                Picture = x.product.Picture,
                Description = x.product.Description,
                Price = x.product.Price,
                Unit = x.product.Unit.ToString(),
                VAT = x.product.VAT,
                CreateDate = x.product.CreateDate
            })
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Product), sku));
    }

    public async Task<Product> GetProductForSalesOrderAsync(Guid id)
    {
        var result = await _context.Products
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new Product
            {
                Id = x.Id,
                Price = x.Price,
            })
            .FirstOrDefaultAsync();
        return result ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Product), id));
    }

    public async Task<HashSet<string>> GetProductSkusAsync()
    {
        return (await _context.Products
            .Select(p => p.SKU.ToLower().Trim())
            .ToListAsync())
            .ToHashSet();
    }

    public async Task<HashSet<Guid>> GetProductIdsAsync()
    {
        return (await _context.Products
            .Select(p => p.Id)
            .ToListAsync())
            .ToHashSet();
    }

    public async Task<QueryResult<LowStockModel>> GetLowStockProductsAsync(Guid storeId, QueryInfo queryInfo)
    {
        var query = _context.Products
             .AsNoTracking()
             .Join(
                 _context.Inventories.AsNoTracking(),
                 product => product.Id,
                 inventory => inventory.ProductId,
                 (product, inventory) => new { product, inventory }
             )
             .Where(x =>
                 x.inventory.StoreId == storeId &&
                 EF.Functions.Collate(x.product.Name, "Latin1_General_CI_AI")
                     .Contains(queryInfo.SearchText ?? "") &&
                 x.product.IsActive == queryInfo.IsActive
             );

        int totalCount = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalCount = await query.CountAsync();
        }

        var pagedEntities = await query
            .OrderBy(x => x.inventory.Quantity)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(x => new LowStockModel
            {
                Name = x.product.Name,
                Quantity = x.inventory.Quantity,
                Price = x.product.Price,
                Code = x.product.SKU,
                Unit = x.product.Unit.ToString(),
                Status = x.inventory.Quantity >= 30 ? "Nhiều hàng"
                         : x.inventory.Quantity >= 15 ? "Trung bình"
                         : x.inventory.Quantity >= 5 ? "Thấp"
                         : "Rất thấp"
            })
            .ToListAsync();

        return new QueryResult<LowStockModel>
        {
            Data = pagedEntities,
            TotalCount = totalCount
        };
    }

    public async Task<Product?> GetProductForStaffOrderAsync(Guid productId, Guid storeId)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == productId && p.IsActive)
            .GroupJoin(
                _context.Inventories.Where(i => i.StoreId == storeId),
                p => p.Id,
                i => i.ProductId,
                (p, inventories) => new { p, inventories }
            )
            .SelectMany(
                x => x.inventories.DefaultIfEmpty(),
                (x, inv) => new Product
                {
                    Id = x.p.Id,
                    Name = x.p.Name,
                    Price = x.p.Price,
                    Unit = x.p.Unit.ToString(),
                    IsActive = x.p.IsActive,
                    Quantity = inv != null ? inv.Quantity : 0  // tồn kho tại cửa hàng này
                }
            )
            .FirstOrDefaultAsync();
    }
}
