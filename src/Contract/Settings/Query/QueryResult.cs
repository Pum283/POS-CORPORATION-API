namespace Contract;

public class QueryResult<TDomain>
{
    public IEnumerable<TDomain> Data { get; set; } = [];
    public int TotalCount { get; set; }
}
