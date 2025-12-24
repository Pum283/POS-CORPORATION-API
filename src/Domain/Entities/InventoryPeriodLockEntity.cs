namespace Domain;

[Table("InventoryPeriodLocks")]
public class InventoryPeriodLockEntity : EntityBase<Guid>
{
    public Guid StoreId { get; set; }

    public Guid? ProductId { get; set; }
    public ProductEntity? Product { get; set; }

    public Guid? IngredientId { get; set; }
    public IngredientEntity? Ingredient { get; set; } 

    public decimal OpeningQuantity { get; set; }  
    public decimal ClosingQuantity { get; set; }
    public InventoryEditStatusType EditStatus { get; set; } = InventoryEditStatusType.NotEdited;
}

