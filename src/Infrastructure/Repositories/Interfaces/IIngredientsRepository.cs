namespace Infrastructure;

public interface IIngredientsRepository : IRepositoryBase<IngredientEntity>
{
    Task<QueryResult<Ingredient>> GetIngredientsWithPagingAsync(QueryInfo queryInfo);
    Task<QueryResult<IngredientsInventoryReportModel>> GetStoreIngredientsPagedAsync(QueryInfo queryInfo, Guid storeId, DateTime? date = null);
    Task<HashSet<string>> GetIngredientCodesAsync();
    Task<HashSet<Guid>> GetIngredientIdsAsync();
    Task<Ingredient?> GetIngredientByCodeAsync(string code);
    Task<QueryResult<LowStockModel>> GetLowStockIngredientsAsync(Guid storeId, QueryInfo queryInfo);
}
