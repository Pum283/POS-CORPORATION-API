namespace Domain;

[Table("Ingredients")]
public class IngredientEntity : EntityBase<Guid>
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public UnitType Unit { get; set; } = default!;
    public ICollection<ProductIngredientEntity> ProductIngredients { get; set; } = new List<ProductIngredientEntity>();
    public ICollection<InventoryPeriodLockEntity> InventoryShiftSnapshots { get; set; } = new List<InventoryPeriodLockEntity>();
}
