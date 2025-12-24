namespace Domain;

public class CreatePurchaseOrderModel
{
    public Guid? SupplierId { get; set; }
    public Guid StoreId { get; set; }   
    public decimal TotalAmount { get; set; }
    public OrderStatusType Status { get; set; }
    public List<CreatePurchaseOrderItem>? PurchaseOrderItem { get; set; }
}
