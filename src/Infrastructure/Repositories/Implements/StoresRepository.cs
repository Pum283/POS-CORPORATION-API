namespace Infrastructure;

[RegisterService(typeof(IStoresRepository))]
public class StoresRepository(EfApplicationDbContext context,
    IMapper _mapper) : RepositoryBase<StoreEntity>(context), IStoresRepository
{
    private readonly EfApplicationDbContext _context = context;

    public async Task<QueryResult<Store>> GetStoresWithPagingAsync(QueryInfo queryInfo)
    {
        var query = _context.Stores.AsNoTracking()
             .Where(x => EF.Functions.Collate(x.Address, "Latin1_General_CI_AI").Contains(queryInfo.SearchText ?? ""));

        var pageEntities = await query
            .OrderByDescending(x => x.CreateDate)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(x => new Store
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                Phone = x.Phone
            })
            .ToListAsync();

        int totalCount = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalCount = await query.CountAsync();
        }

        return new QueryResult<Store>
        {
            Data = pageEntities,
            TotalCount = totalCount
        };
    }

    public async Task<QueryResult<Store>> GetStoresWithoutAccountAsync(QueryInfo queryInfo)
    {
        var search = queryInfo.SearchText ?? "";

        var query = from s in _context.Stores.AsNoTracking()
                    join sa in _context.Users on s.Id equals sa.StoreId into g
                    from sa in g.DefaultIfEmpty()
                    where sa == null
                          && EF.Functions.Collate(s.Address, "Latin1_General_CI_AI").Contains(search)
                    select s;

        var pageEntities = await query
            .OrderByDescending(x => x.CreateDate)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(x => new Store
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                Phone = x.Phone
            })
            .ToListAsync();

        int totalCount = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalCount = await query.CountAsync();
        }

        return new QueryResult<Store>
        {
            Data = pageEntities,
            TotalCount = totalCount
        };
    }

    public async Task<Store> GetById(Guid id)
    {
        var entity = await _context.Stores.Select(x => new {x.Id, x.Name}).FirstOrDefaultAsync(x => x.Id == id);
        return _mapper.Map<Store>(entity);
    }

    public async Task<List<Guid>> GetStoresByProductIdAsync(Guid id)
    {
        return await _context.Inventories
            .Where(x => x.ProductId == id)
            .Select(x => x.StoreId)
            .ToListAsync();
    }
}
