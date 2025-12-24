namespace Domain;

public class CreatePurchaseOrderItem
{
    public Guid PurchaseOrderId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public int UnitCost { get; set; }
}
