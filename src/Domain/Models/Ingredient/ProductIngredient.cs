namespace Domain;

public class ProductIngredient
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Ingredient? Ingredient { get; set; }
    public decimal Quantity { get; set; }
}
