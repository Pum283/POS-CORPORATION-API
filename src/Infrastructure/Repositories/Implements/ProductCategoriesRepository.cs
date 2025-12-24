namespace Infrastructure;

[RegisterService(typeof(IProductCategoriesRepository))]
public class ProductCategoriesRepository(
    EfApplicationDbContext context,
    IMapper _mapper)
    : RepositoryBase<ProductCategoryEntity>(context), IProductCategoriesRepository
{
    private readonly EfApplicationDbContext _context = context;
    public async Task<QueryResult<ProductCategory>> GetProductCategoriesWithPagingAsync(QueryInfo queryInfo)
    {
        var query = _context.ProductCategoies
            .AsNoTracking()
            .Where(x => EF.Functions.Collate(x.Name, "Latin1_General_CI_AI").Contains(queryInfo.SearchText ?? ""))
            .OrderBy(x => x.CreateDate);

        var pagedEntities = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(x => new ProductCategoryEntity
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                CreateDate = x.CreateDate
            })
            .ToListAsync();

        int totalCount = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalCount = await query.CountAsync();
        }

        return new QueryResult<ProductCategory>
        {
            Data = _mapper.Map<List<ProductCategory>>(pagedEntities),
            TotalCount = totalCount
        };
    }
    public async Task<List<ProductCategoriesIncludeProductGroupsModel>> GetProductCategoriesIncludeProductGroupsWithPagingAsync()
    {
        var entity = from pc in _context.ProductCategoies.AsNoTracking()
                     join pg in _context.ProductGroups.AsNoTracking() on pc.Id equals pg.ProductCategoryId into pgGroup
                     select new ProductCategoriesIncludeProductGroupsModel
                     {
                         Id = pc.Id,
                         Name = pc.Name,
                         Description = pc.Description,
                         ProductGroups = pc.ProductGroup != null ? pc.ProductGroup.Select(ProductGroup => new ProductGroup
                         {
                             Id = ProductGroup.Id,
                             Name = ProductGroup.Name,
                             Description = ProductGroup.Description
                         }).ToList() : new List<ProductGroup>()
                     };
        return await entity.ToListAsync() ?? new List<ProductCategoriesIncludeProductGroupsModel>();
    }
}
