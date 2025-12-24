namespace Domain;

public class InventoryPeriodLockModel
{
    public Guid Id { get; set; }
    [JsonIgnore]
    public Guid StoreId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? IngredientId { get; set; }
    public decimal OpeningQuantity { get; set; }
    public decimal ClosingQuantity { get; set; }
    public InventoryEditStatusType EditStatus { get; set; }
}
