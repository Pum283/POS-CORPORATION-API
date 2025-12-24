namespace Domain;

public class PurchaseOrder
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public Guid StoreId { get; set; }
    public Store? Store { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatusType Status { get; set; }
    public List<PurchaseOrderItem>? PurchaseOrderItem { get; set; }
}
