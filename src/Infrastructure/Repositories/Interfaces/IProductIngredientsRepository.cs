namespace Infrastructure;

public interface IProductIngredientsRepository : IRepositoryBase<ProductIngredientEntity>
{
    Task<QueryResult<ProductIngredient>> GetIngredientsForProductAsync(Guid productId, QueryInfo queryInfo);
}
