namespace Infrastructure;

[RegisterService(typeof(IIngredientsRepository))]
public class IngredientsRepository(EfApplicationDbContext context) : RepositoryBase<IngredientEntity>(context), IIngredientsRepository
{
    private readonly EfApplicationDbContext _context = context;
    public async Task<QueryResult<Ingredient>> GetIngredientsWithPagingAsync(QueryInfo queryInfo)
    {
        var ingredient = _context.Ingredients
            .Where(x => EF.Functions.Collate(x.Name, "Latin1_General_CI_AI").Contains(queryInfo.SearchText ?? "") ||
                        EF.Functions.Collate(x.Code, "Latin1_General_CI_AI").Contains(queryInfo.SearchText ?? ""))
            .Select(x => new Ingredient
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Unit = x.Unit.ToString(),
            });

        var pageEntities = await ingredient
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        var totalCount = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalCount = ingredient.Count();
        }
        return new QueryResult<Ingredient>
        {
            Data = pageEntities,
            TotalCount = totalCount
        };
    }
    public async Task<QueryResult<IngredientsInventoryReportModel>> GetStoreIngredientsPagedAsync(QueryInfo queryInfo, Guid storeId, DateTime? date = null)
    {
        var todayStart = await _context.PeriodLocks
            .Where(x => x.StoreId == storeId && x.Status == ShiftPeriodStatusType.Started)
            .Select(x => x.CreateDate)
            .FirstOrDefaultAsync();
        var tomorrowStart = todayStart.AddHours(12);

        // 1. Query phân trang tồn kho + sản phẩm
        var ingredientInventoryQuery = (
            from ii in _context.IngredientInventories.AsNoTracking()
            join i in _context.Ingredients.AsNoTracking() on ii.IngredientId equals i.Id
            where ii.StoreId == storeId
            select new { IngredientInventory = ii, Ingredient = i }
        );

        var totalCount = await ingredientInventoryQuery.CountAsync();

        var ingredientsInventorie = await ingredientInventoryQuery
            .OrderBy(x => x.Ingredient.Name)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        var ingredientIds = ingredientsInventorie
            .Select(x => x.Ingredient.Id)
            .ToList();

        // 2. Opening quantity
        var originalQuantities = await _context.InventoryPeriodLocks
            .Where(x => ingredientIds.Contains(x.IngredientId!.Value) && x.StoreId == storeId)
            .GroupBy(x => x.IngredientId)
            .Select(g => new
            {
                IngredientId = g.Key!.Value,
                OpeningQuantity = g.Max(x => x.OpeningQuantity)
            })
            .ToDictionaryAsync(x => x.IngredientId, x => x.OpeningQuantity);

        // 3. Sold today
        var soldToday = await (
            from so in _context.SalesOrders.AsNoTracking()
            join soi in _context.SalesOrderItems.AsNoTracking() on so.Id equals soi.SalesOrderId
            join pi in _context.ProductIngredients.AsNoTracking() on soi.ProductId equals pi.ProductId
            where so.StoreId == storeId
                && so.OrderStatus == OrderStatusType.Confirmed
                && so.ModifiedDate >= todayStart && so.ModifiedDate < tomorrowStart
                && ingredientIds.Contains(pi.IngredientId)
            group new { soi, pi } by pi.IngredientId into g
            select new
            {
                IngredientId = g.Key,
                Quantity = g.Sum(x => x.soi.Quantity * x.pi.Quantity)
            }
            ).ToDictionaryAsync(x => x.IngredientId, x => x.Quantity);

        // 4. Input quantity
        var inputQuantities = await _context.DocumentSuppliers
            .Where(x => x.StoreId == storeId
                        && x.TransType == TransType.GoodsReceipt
                        && x.ModifiedDate >= todayStart && x.ModifiedDate < tomorrowStart
                        && x.IngredientId.HasValue
                        && ingredientIds.Contains(x.IngredientId!.Value))
            .GroupBy(x => x.IngredientId)
            .Select(g => new { IngredientId = g.Key!.Value, Quantity = g.Sum(x => x.Quantity) })
            .ToDictionaryAsync(x => x.IngredientId, x => x.Quantity);

        // 5. Canceled quantity
        var canceledQuantities = await _context.DocumentSuppliers
            .Where(x => x.StoreId == storeId
                        && x.TransType == TransType.CanceledProduct
                        && x.ModifiedDate >= todayStart && x.ModifiedDate < tomorrowStart
                        && x.IngredientId.HasValue
                        && ingredientIds.Contains(x.IngredientId!.Value))
            .GroupBy(x => x.IngredientId)
            .Select(g => new { IngredientId = g.Key!.Value, Quantity = g.Sum(x => x.Quantity) })
            .ToDictionaryAsync(x => x.IngredientId, x => x.Quantity);

        // 6. Tổng hợp kết quả
        var summaries = ingredientsInventorie.Select(x =>
        {
            var ingredientId = x.IngredientInventory.IngredientId;

            var original = originalQuantities.TryGetValue(ingredientId, out var oq) ? oq : 0;
            var sold = soldToday.TryGetValue(ingredientId, out var s) ? s : 0;
            var input = inputQuantities.TryGetValue(ingredientId, out var i) ? i : 0;
            var canceled = canceledQuantities.TryGetValue(ingredientId, out var c) ? c : 0;

            return new IngredientsInventoryReportModel
            {
                Code = x.Ingredient.Code,
                Name = x.Ingredient.Name,
                Unit = x.Ingredient.Unit.ToString(),
                OriginalQuantity = original,
                BuyQuantity = sold,
                InputQuantity = input,
                Quantity = original - sold - canceled,
            };
        }).ToList();

        // 7. Trả kết quả
        return new QueryResult<IngredientsInventoryReportModel>
        {
            Data = summaries,
            TotalCount = totalCount
        };
    }
    public async Task<HashSet<string>> GetIngredientCodesAsync()
    {
        return (await _context.Ingredients
            .Select(x => x.Code.Trim().ToLower())
            .ToListAsync())
            .ToHashSet();
    }

    public async Task<HashSet<Guid>> GetIngredientIdsAsync()
    {
        return (await _context.IngredientInventories
            .AsNoTracking()
            .Select(x => x.IngredientId)
            .ToListAsync())
            .ToHashSet();
    }

    public async Task<Ingredient?> GetIngredientByCodeAsync(string code)
    {
        var ingredientEntity = await _context.Ingredients
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Code.Trim().ToLower() == code.Trim().ToLower());

        if (ingredientEntity == null)
            return null;

        return new Ingredient
        {
            Id = ingredientEntity.Id,
            Name = ingredientEntity.Name,
            Code = ingredientEntity.Code,
            Unit = ingredientEntity.Unit.ToString()
        };
    }

    public async Task<QueryResult<LowStockModel>> GetLowStockIngredientsAsync(Guid storeId, QueryInfo queryInfo)
    {
        var query = _context.Ingredients
             .AsNoTracking()
             .Join(
                 _context.IngredientInventories.AsNoTracking(),
                 ingredient => ingredient.Id,
                 inventory => inventory.IngredientId,
                 (ingredient, inventory) => new { ingredient, inventory }
             )
             .Where(x =>
                 x.inventory.StoreId == storeId &&
                 EF.Functions.Collate(x.ingredient.Name, "Latin1_General_CI_AI")
                     .Contains(queryInfo.SearchText ?? "")
             );

        int totalCount = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalCount = await query.CountAsync();
        }

        var entities = await query
            .OrderBy(x => x.inventory.Quantity)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(x => new LowStockModel
            {
                Name = x.ingredient.Name,
                Quantity = x.inventory.Quantity,
                Code = x.ingredient.Code,
                Unit = x.ingredient.Unit.ToString(),
                Status = x.inventory.Quantity >= 30 ? "Nhiều hàng"
                         : x.inventory.Quantity >= 15 ? "Trung bình"
                         : x.inventory.Quantity >= 5 ? "Thấp"
                         : "Rất thấp"
            })
            .ToListAsync();

        return new QueryResult<LowStockModel>
        {
            Data = entities,
            TotalCount = totalCount
        };
    }
}
