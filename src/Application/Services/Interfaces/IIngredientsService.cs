namespace Application;

public interface IIngredientsService
{
    Task<QueryResult<Ingredient>> GetIngredientsWithPagingAsync(QueryInfo queryInfo);
    Task<QueryResult<IngredientsInventoryReportModel>> GetStoreIngredientsPagedAsync(QueryInfo queryInfo, Guid storeId, DateTime? date = null);
    Task<HashSet<string>> GetCodesAsync();
    Task<Ingredient> GetIngredientByIdAsync(Guid id);
    Task<Ingredient> GetIngredientByCodeAsync(string code);
    Task<QueryResult<LowStockModel>> GetLowStockIngredientsAsync(Guid storeId, QueryInfo queryInfo);
    Task<OperationResult> CreateIngredientAsync(CreateOrUpdateIngredientModel model);
    Task<OperationResult> CreateIngredientsAsync(List<CreateOrUpdateIngredientModel> model);
    Task<OperationResult> UpdateIngredientAsync(Guid id, CreateOrUpdateIngredientModel model);
    Task<OperationResult> DeleteIngredientAsync(Guid id);
    Task<(bool IsValid, string? SuggestCode, string? Reason)> CheckIngredientCodeAsync(string code);
}
