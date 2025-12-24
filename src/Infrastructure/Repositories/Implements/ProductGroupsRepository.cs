namespace Infrastructure;

[RegisterService(typeof(IProductGroupsRepository))]
public class ProductGroupsRepository(
    EfApplicationDbContext context,
    IMapper _mapper) : RepositoryBase<ProductGroupEntity>(context), IProductGroupsRepository
{
    private readonly EfApplicationDbContext _context = context;


    public async Task<QueryResult<ProductGroup>> GetProductGroupsWithPaging(QueryInfo queryInfo)
    {
        var query = _context.ProductGroups
            .AsNoTracking()
            .Where(x => EF.Functions.Collate(x.Name, "Latin1_General_CI_AI").Contains(queryInfo.SearchText ?? ""))
            .OrderByDescending(x => x.CreateDate);

        var pagedEntities = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(x => new ProductGroupEntity
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ProductCategoryId = x.ProductCategoryId,
                CreateDate = x.CreateDate
            })
            .ToListAsync();


        int totalCount = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalCount = await query.CountAsync();
        }
        return new QueryResult<ProductGroup>
        {
            Data = _mapper.Map<List<ProductGroup>>(pagedEntities),
            TotalCount = totalCount
        };
    }
    public async Task<QueryResult<Product>> GetProductsByGroupIdWithPagingAsync(Guid storeId, Guid productGroupId, QueryInfo queryInfo)
    {
        IQueryable<Product> query;

        if (storeId == Guid.Empty)
        {
            query = _context.Products
                .AsNoTracking()
                .Where(p =>
                    p.ProductGroupId == productGroupId &&
                    EF.Functions.Collate(p.Name, "Latin1_General_CI_AI")
                        .Contains(queryInfo.SearchText ?? "") &&
                    p.IsActive == queryInfo.IsActive
                )
                .OrderByDescending(p => p.CreateDate)
                .Select(p => new Product
                {
                    Id = p.Id,
                    ProductGroup = new ProductGroupBasicModel
                    {
                        Id = p.ProductGroup!.Id,
                        Name = p.ProductGroup!.Name,
                    },
                    Name = p.Name,
                    Cost = p.Cost,
                    SKU = p.SKU,
                    Picture = p.Picture,
                    Description = p.Description,
                    Price = p.Price,
                    IsActive = p.IsActive,
                    Unit = p.Unit.ToString(),
                    VAT = p.VAT,
                    Quantity = 0, 
                    CreateDate = p.CreateDate
                });
        }
        else
        {
            query = _context.Products
                .AsNoTracking()
                .Join(
                    _context.Inventories.AsNoTracking(),
                    product => product.Id,
                    inventory => inventory.ProductId,
                    (product, inventory) => new { product, inventory }
                )
                .Where(x =>
                    x.product.ProductGroupId == productGroupId &&
                    x.inventory.StoreId == storeId &&
                    EF.Functions.Collate(x.product.Name, "Latin1_General_CI_AI")
                        .Contains(queryInfo.SearchText ?? "") &&
                    x.product.IsActive == queryInfo.IsActive
                )
                .OrderByDescending(x => x.product.CreateDate)
                .Select(x => new Product
                {
                    Id = x.product.Id,
                    ProductGroup = new ProductGroupBasicModel
                    {
                        Id = x.product.ProductGroup!.Id,
                        Name = x.product.ProductGroup!.Name,
                    },
                    Name = x.product.Name,
                    Cost = x.product.Cost,
                    SKU = x.product.SKU,
                    Picture = x.product.Picture,
                    Description = x.product.Description,
                    Price = x.product.Price,
                    IsActive = x.product.IsActive,
                    Unit = x.product.Unit.ToString(),
                    VAT = x.product.VAT,
                    Quantity = x.inventory.Quantity, 
                    CreateDate = x.product.CreateDate
                });
        }

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

    public async Task<QueryResult<ProductGroup>> GetProductGroupByCategoryIdWithPaging(Guid categoryId, QueryInfo queryInfo)
    {
        var query = _context.ProductGroups
            .AsNoTracking()
            .Where(x => x.ProductCategoryId == categoryId)
            .Where(x => EF.Functions.Collate(x.Name, "Latin1_General_CI_AI").Contains(queryInfo.SearchText ?? ""))
            .OrderByDescending(x => x.CreateDate);

        var pagedEntities = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(x => new ProductGroupEntity
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

        return new QueryResult<ProductGroup>
        {
            Data = _mapper.Map<List<ProductGroup>>(pagedEntities),
            TotalCount = totalCount
        };
    }
}
