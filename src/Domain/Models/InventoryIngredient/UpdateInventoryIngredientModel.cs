namespace Domain;

public class UpdateInventoryIngredientModel
{
    public Guid StoreId { get; set; }
    public Guid IngredientId { get; set; }
    public decimal Quantity { get; set; }
}
