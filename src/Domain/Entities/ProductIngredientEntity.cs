namespace Domain;

[Table("ProductIngredients")]
public class ProductIngredientEntity : EntityBase<Guid>
{
    public Guid ProductId { get; set; }
    public ProductEntity Product { get; set; } = default!;
    public Guid IngredientId { get; set; }
    public IngredientEntity Ingredient { get; set; } = default!;
    public decimal Quantity { get; set; }
}
