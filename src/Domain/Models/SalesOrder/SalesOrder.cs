namespace Domain;

public class SalesOrder
{
    public Guid Id { get; set; }
    public string? OrderCode { get; set; }
    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public Guid StoreId { get; set; }
    public Store? Store { get; set; }
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public string? PaymentMethod { get; set; }
    public decimal PaidAmount { get; set; }
    public List<SalesOrderItem> SalesOrderItem { get; set; } = [];
    public List<SalesOrderPromotion> SalesOrderPromotion { get; set; } = [];
}
