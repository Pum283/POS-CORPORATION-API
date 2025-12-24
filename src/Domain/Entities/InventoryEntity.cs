namespace Domain;

[Table("Inventories")]
public class InventoryEntity : EntityBase<Guid>
{
    public Guid StoreId { get; set; }
    public StoreEntity? Store { get; set; }
    public Guid ProductId { get; set; }
    public ProductEntity? Product { get; set; }
    public decimal Quantity { get; set; } 
}
