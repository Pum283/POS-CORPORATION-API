namespace Domain;

[Table("PurchaseOrderItems")]
public class PurchaseOrderItemEntity : EntityBase<Guid>
{
    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrderEntity? PurchaseOrder { get; set; }
    public Guid ProductId { get; set; }
    public ProductEntity? Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
}
