namespace Domain;

public class UpdateInventoryQuantityLockModel
{
    public Guid StoreId { get; set; }
    public Guid ProductId { get; set; }
    public decimal ClosingQuantity { get; set; }
}
