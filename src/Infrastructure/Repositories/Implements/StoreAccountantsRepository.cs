namespace Infrastructure;

[RegisterService(typeof(IStoreAccountantsRepository))]
public class StoreAccountantsRepository(
    EfApplicationDbContext context) : RepositoryBase<StoreAccountantEntity>(context), IStoreAccountantsRepository
{
    private readonly EfApplicationDbContext _context = context;
    //public async Task<QueryResult<StoreAccountListModel>> GetStoreAccountantsWithPagingAsync(QueryInfo queryInfo)
    //{
    //    var query = from storeAcc in _context.StoreAccountants.AsNoTracking()
    //                join store in _context.Stores.AsNoTracking()
    //                 on storeAcc.StoreId equals store.Id
    //                select new StoreAccountListModel
    //                {
    //                    Id = storeAcc.Id,
    //                    AccountantId = storeAcc.AccountantId,
    //                    Stores = new List<StoreModel>
    //                    {
    //                        new StoreModel
    //                        {
    //                            Id = store.Id,
    //                            Name = store.Name,
    //                        }
    //                    }
    //                };
    //    var entities = await query
    //        .Skip(queryInfo.Skip)
    //        .Take(queryInfo.Top)
    //        .ToListAsync();


    //    return new QueryResult<StoreAccountListModel>
    //    {
    //        Data = entities,
    //        TotalCount = queryInfo.NeedTotalCount
    //            ? await query.CountAsync()
    //            : 0
    //    };
    //}
    public async Task<QueryResult<StoreAccountListModel>> GetStoreAccountantsWithPagingAsync(QueryInfo queryInfo)
    {
        var query = from storeAcc in _context.StoreAccountants.AsNoTracking()
                    join store in _context.Stores.AsNoTracking()
                        on storeAcc.StoreId equals store.Id
                    group new { store, storeAcc } by storeAcc.AccountantId into g
                    select new StoreAccountListModel
                    {
                        AccountantId = g.Key,
                        Id = g.Select(x => x.storeAcc.Id).FirstOrDefault(),
                        Stores = g.Select(x => new StoreModel
                        {
                            Id = x.store.Id,
                            Name = x.store.Name
                        }).ToList()
                    };

        var totalCount = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;

        var entities = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        return new QueryResult<StoreAccountListModel>
        {
            Data = entities,
            TotalCount = totalCount
        };
    }


    public async Task<QueryResult<StoreAccountantModel>> GetStoreOfAccountantsWithPagingAsync(QueryInfo queryInfo, Guid accountantId)
    {
        var query = from storeAcc in _context.StoreAccountants.AsNoTracking()
                    where storeAcc.AccountantId == accountantId
                    join store in _context.Stores.AsNoTracking()
                     on storeAcc.StoreId equals store.Id
                    select new StoreAccountantModel
                    {
                        Id = storeAcc.Id,
                        StoreId = store.Id,
                        StoreName = storeAcc.Store.Name,
                        AccountantId = storeAcc.AccountantId,
                    };
        var entities = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();


        return new QueryResult<StoreAccountantModel>
        {
            Data = entities,
            TotalCount = queryInfo.NeedTotalCount
                ? await query.CountAsync()
                : 0
        };
    }

    public async Task<List<StoreAccountantModel>?> GetStoresOfAccountantByIdAsync(Guid accountantId)
    {
        return await (from storeAcc in _context.StoreAccountants.AsNoTracking()
                      where storeAcc.AccountantId == accountantId
                      join store in _context.Stores.AsNoTracking()
                     on storeAcc.StoreId equals store.Id
                      select new StoreAccountantModel
                      {
                          Id = storeAcc.Id,
                          StoreId = store.Id,
                          StoreName = storeAcc.Store.Name,
                          AccountantId = storeAcc.AccountantId,
                      }).ToListAsync();
    }
}
