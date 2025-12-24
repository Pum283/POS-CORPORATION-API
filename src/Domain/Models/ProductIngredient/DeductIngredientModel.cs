namespace Domain;

public class DeductIngredientModel
{
    public Guid ProductId { get; set; }
    public Guid StoreId { get; set; }
    public decimal Quantity { get; set; }
}
