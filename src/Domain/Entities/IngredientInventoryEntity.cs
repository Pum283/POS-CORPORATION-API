namespace Domain;

[Table("IngredientInventories")]
public class IngredientInventoryEntity : EntityBase<Guid>
{
    public Guid StoreId { get; set; }
    public StoreEntity? Store { get; set; }

    public Guid IngredientId { get; set; }
    public IngredientEntity? Ingredient { get; set; }

    public decimal Quantity { get; set; } 
}
