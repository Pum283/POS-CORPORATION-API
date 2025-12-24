namespace Domain;

public class UpdateIngredientInventoryQuantityLockModel
{
    public Guid StoreId { get; set; }
    public Guid IngredientId { get; set; }
    public decimal ClosingQuantity { get; set; }
}
