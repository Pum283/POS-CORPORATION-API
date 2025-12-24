namespace Domain;

public class SalesOrderModel
{
    public Guid Id { get; set; }
    public string? OrderCode { get; set; }
    public string? CustomerName { get; set; }
    public string? UserName { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public string? PaymentMethod { get; set; }
    public decimal PaidAmount { get; set; }
    public string? CreateDate { get; set; }
    public List<SalesOrderItemModel> SalesOrderItems { get; set; } = [];
}
