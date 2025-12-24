namespace Domain;

[Table("PurchaseOrders")]
public class PurchaseOrderEntity : EntityBase<Guid>
{
    public Guid? SupplierId { get; set; }
    public SupplierEntity? Supplier { get; set; }
    public Guid StoreId { get; set; }
    public StoreEntity? Store { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatusType Status { get; set; }

    public ICollection<PurchaseOrderItemEntity>? PurchaseOrderItem { get; set; }
}
