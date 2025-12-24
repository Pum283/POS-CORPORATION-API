namespace Domain;

public class SalesOrderOfCustomerModel
{
    public Guid OrderId { get; set; }
    public Guid? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    public string OrderCode { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string? PaymentMethod { get; set; }
    public string Time { get; set; } = string.Empty;
    public string? ShippingType { get; set; }
}
