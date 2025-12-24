namespace Domain;

public class AddIngredientForProductModel
{
    public Guid ProductId { get; set; }
    public Guid IngredientId { get; set; }
    public decimal Quantity { get; set; }
}
