namespace Infrastructure;

[RegisterService(typeof(IPromotionsRepository))]
public class PromotionsRepository(EfApplicationDbContext context) : RepositoryBase<PromotionEntity>(context), IPromotionsRepository
{
    private readonly EfApplicationDbContext _context = context;

    public async Task<QueryResult<Promotion>> GetPromotionsAsync(QueryInfo queryInfo, GetPromotionsQuery promotionsQuery)
    {
        var queryBase = _context.Promotions.AsQueryable();

        if (promotionsQuery.StoreId != Guid.Empty)
        {
            queryBase = queryBase.Where(p => p.StoreScope == StoreTargetScopeType.ByStore);
        }
        if (!string.IsNullOrEmpty(promotionsQuery.PromotionType) &&
            Enum.TryParse(promotionsQuery.PromotionType, out PromotionType promotionType))
        {
            queryBase = queryBase.Where(p => p.Type == promotionType);
        }
        if (promotionsQuery.StartDate.HasValue && promotionsQuery.EndDate.HasValue)
        {
            queryBase = queryBase.Where(p =>
                        p.StartDate >= promotionsQuery.StartDate.Value &&
                        p.EndDate <= promotionsQuery.EndDate.Value);
        }

        //queryBase = queryBase.Where(p => p.IsActive == queryInfo.IsActive);

        var promotions = await queryBase
            .OrderByDescending(p => p.CreateDate)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(p => new Promotion
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Type = p.Type,
                Value = p.Value,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                UsageLimit = p.UsageLimit ?? 0,
                UsageCount = p.UsageCount,
                IsActive = p.IsActive,

                StoreScope = _context.PromotionStores
                    .Where(ps => ps.PromotionId == p.Id)
                    .Join(_context.Stores,
                          ps => ps.StoreId,
                          s => s.Id,
                          (ps, s) => s.Name)
                    .Distinct()
                    .ToList(),

                ProductScope = _context.PromotionProducts
                    .Where(pp => pp.PromotionId == p.Id)
                    .Join(_context.Products,
                          pp => pp.ProductId,
                          pr => pr.Id,
                          (pp, pr) => pr.Name)
                    .Distinct()
                    .ToList(),

                ProductGroupScope = _context.PromotionProductGroups
                    .Where(pp => pp.PromotionId == p.Id)
                    .Join(_context.ProductGroups,
                          pp => pp.ProductGroupId,
                          pr => pr.Id,
                          (pp, pr) => pr.Name)
                    .Distinct()
                    .ToList(),
            })
            .ToListAsync();

        int totalCount = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalCount = await queryBase.CountAsync();
        }

        return new QueryResult<Promotion>
        {
            Data = promotions,
            TotalCount = totalCount
        };
    }

    public async Task<List<PromotionOfProductModel>> GetPromotionsOfProductWithPagingAsync(Guid productId, Guid storeId)
    {
        var now = DateTime.UtcNow;

        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId);

        var productGroupId = product!.ProductGroupId;
        var originalPrice = product.Price;

        var query = _context.Promotions
            .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now)
            .Select(p => new
            {
                Promotion = p,
                Category =
                    p.PromotionProducts.Any(pp => pp.ProductId == productId) ? "ByProduct" :
                    (productGroupId != null && p.PromotionProductGroups.Any(pg => pg.ProductGroupId == productGroupId)) ? "ByGroup" :
                    (storeId != Guid.Empty && p.PromotionStores.Any(ps => ps.StoreId == storeId)) ? "ByStore" :
                    null
            })
            .Where(x => x.Category != null)
            .Select(x => new
            {
                x.Promotion.Id,
                x.Promotion.Code,
                x.Promotion.Name,
                x.Promotion.Type,
                x.Promotion.Value,
                Category = x.Category!,
                DiscountAmount = x.Promotion.Type == PromotionType.Percentage
                    ? originalPrice * x.Promotion.Value / 100
                    : x.Promotion.Value
            })
            .OrderByDescending(x => x.DiscountAmount)
            .Take(10);

        var data = await query.ToListAsync();

        var grouped = data
            .GroupBy(x => x.Category)
            .Select(g => new PromotionOfProductModel
            {
                PromotionCategory = g.Key,
                PromotionDetailOfProductModels = g.Select(p => new PromotionDetailOfProductModel
                {
                    PromotionId = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    Type = p.Type,
                    Value = p.Value,
                    DiscountAmount = p.DiscountAmount
                }).ToList()
            })
            .ToList();

        return grouped;
    }
    public async Task<Promotion> GetByIdAsync(Guid id)
    {
        var promo = await _context.Promotions
            .Where(x => x.Id == id)
            .Select(x => new Promotion
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Type = x.Type,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsActive = x.IsActive,

                StoreScope = (from ps in _context.PromotionStores
                              join s in _context.Stores on ps.StoreId equals s.Id
                              where ps.PromotionId == id
                              select s.Name).Distinct().ToList(),

                ProductScope = (from pp in _context.PromotionProducts
                                join pr in _context.Products on pp.ProductId equals pr.Id
                                where pp.PromotionId == id
                                select pr.Name).Distinct().ToList(),
                ProductGroupScope = (from pp in _context.PromotionProductGroups
                                     join pr in _context.ProductGroups on pp.ProductGroupId equals pr.Id
                                     where pp.PromotionId == id
                                     select pr.Name).Distinct().ToList(),
            })
            .FirstOrDefaultAsync();
        if (promo == null)
            throw new NotFoundException(
                ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Promotion), id));

        return promo;
    }

    public async Task<PromotionModel> GetPromotionByProductIdAsync(Guid productId, Guid storeId)
    {
        var entity = await _context.PromotionProducts.AsNoTracking()
            .Where(pp => pp.ProductId == productId)
            .Join(
                _context.Products.AsNoTracking(),
                pp => pp.ProductId,
                p => p.Id,
                (pp, p) => new { pp, p }
            )
            .OrderByDescending(x => x.pp.Promotion.Type == PromotionType.Percentage
                ? x.p.Price * x.pp.Promotion.Value / 100   
                : x.pp.Promotion.Value                     
            )
            .Select(x => new PromotionModel
            {
                Id = x.pp.Promotion.Id,
                Code = x.pp.Promotion.Code,
                Name = x.pp.Promotion.Name,
                Type = x.pp.Promotion.Type.ToString(),
                Value = x.pp.Promotion.Value,
                StartDate = x.pp.Promotion.StartDate.ToString("dd-MM-yyyy"),
                EndDate = x.pp.Promotion.EndDate.ToString("dd-MM-yyyy"),
                UsageLimit = x.pp.Promotion.UsageLimit ?? 0,
                UsageCount = x.pp.Promotion.UsageCount,
                IsActive = x.pp.Promotion.IsActive,
                ProductScope = x.pp.Promotion.ProductScope.ToString(),
                StoreScope = x.pp.Promotion.StoreScope.ToString()
            })
            .FirstOrDefaultAsync();

        return entity ?? throw new NotFoundException($"Not found promotion of product with id: {productId}");
    }


    public async Task<PromotionModel> GetPromotionByProductGroupIdAsync(Guid productId, Guid productGroupId, Guid storeId)
    {
        var now = DateTime.UtcNow;

        var entity = await (
            from i in _context.Inventories.AsNoTracking()
            join p in _context.Products on i.ProductId equals p.Id
            join pg in _context.ProductGroups on p.ProductGroupId equals pg.Id
            join pp in _context.PromotionProductGroups.AsNoTracking() on pg.Id equals pp.ProductGroupId
            where i.ProductId == productId
                  && i.StoreId == storeId
                  && pp.ProductGroupId == productGroupId
                  && pp.Promotion.IsActive
                  && pp.Promotion.StartDate <= now
                  && pp.Promotion.EndDate >= now
            orderby (pp.Promotion.Type == PromotionType.Percentage
                        ? (p.Price * pp.Promotion.Value / 100)
                        : pp.Promotion.Value) descending
            select new PromotionModel
            {
                Id = pp.Promotion.Id,
                Code = pp.Promotion.Code,
                Name = pp.Promotion.Name,
                Type = pp.Promotion.Type.ToString(),
                Value = pp.Promotion.Type == PromotionType.Percentage
                    ? (p.Price * pp.Promotion.Value / 100)
                    : pp.Promotion.Value
            }
        ).FirstOrDefaultAsync();

        return entity ?? throw new NotFoundException(
            $"Not found Promotion for Product Group with id {productGroupId} in Store with id {storeId}");
    }
}
