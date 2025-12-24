namespace Infrastructure;

[RegisterService(typeof(IProductIngredientsRepository))]
public class ProductIngredientsRepository(EfApplicationDbContext context) : RepositoryBase<ProductIngredientEntity>(context), IProductIngredientsRepository
{
    private readonly EfApplicationDbContext _context = context;

    public async Task<QueryResult<ProductIngredient>> GetIngredientsForProductAsync(Guid productId, QueryInfo queryInfo)
    {
        var query = _context.ProductIngredients
            .Where(x => x.ProductId == productId)
            .OrderByDescending(x=> x.ModifiedDate)
            .Join(_context.Ingredients,
                pi => pi.IngredientId,
                i => i.Id,
                (pi, i) => new ProductIngredient
                {
                    Id = pi.Id,
                    ProductId = productId,
                    Quantity = pi.Quantity,
                    Ingredient = new Ingredient
                    {
                        Id = i.Id,
                        Code = i.Code,
                        Name = i.Name,
                        Unit = i.Unit.ToString(),
                    }
                });

        var pagedEntites = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        var totalCount = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalCount = await query.CountAsync();
        }

        return new QueryResult<ProductIngredient>
        {
            Data = pagedEntites,
            TotalCount = totalCount,
        };
    }
}
