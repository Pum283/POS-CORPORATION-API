namespace Infrastructure;

[RegisterService(typeof(IOperationMetricsRepository))]
public class OperationMetricsRepository(EfApplicationDbContext context) : RepositoryBase<OperationMetricEntity>(context), IOperationMetricsRepository
{
    private readonly EfApplicationDbContext _context = context;

    public async Task<QueryResult<OperationMetricModel>> GetOperationMetricsAsync(QueryInfo queryInfo, OperationMetricQuery operationMetricQuery)
    {
        var query = _context.OperationMetrics.AsNoTracking()
            .Where(x => x.StoreId == operationMetricQuery.StoreId);

        var entities = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(x => new OperationMetricModel
            {
                Id = x.Id,
                Store = x.Store != null ? new StoreModel
                {
                    Id = x.Store.Id,
                    Name = x.Store.Name
                } : null,
                AppType = x.AppType.ToString(),
                GMV = x.GMV,
                NetSale = x.NetSale,
                OrderCount = x.OrderCount,
                RecordDate = x.RecordDate
            })
            .ToListAsync();
        int totalCount = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalCount = await query.CountAsync();
        }
        return new QueryResult<OperationMetricModel>
        {
            Data = entities,
            TotalCount = totalCount
        };
    }
    public async Task<OperationMetricModel> GetOperationMetricByIdAsync(Guid id)
    {
        return await _context.OperationMetrics.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new OperationMetricModel
            {
                Id = x.Id,
                Store = x.Store != null ? new StoreModel
                {
                    Id = x.Store.Id,
                    Name = x.Store.Name
                } : null,
                AppType = x.AppType.ToString(),
                GMV = x.GMV,
                NetSale = x.NetSale,
                OrderCount = x.OrderCount,
                RecordDate = x.RecordDate
            })
            .FirstOrDefaultAsync() ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(OperationMetricEntity), id));
    }
}
