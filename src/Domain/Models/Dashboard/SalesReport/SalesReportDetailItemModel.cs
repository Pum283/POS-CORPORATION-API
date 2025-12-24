namespace Domain;

public class SalesReportDetailItemModel
{
    public decimal UnitPrice { get; set; }
    public int QuantitySold { get; set; }
    public decimal TotalBeforeDiscount { get; set; }  // = UnitPrice * Quantity + discount nếu có
    public decimal TotalDiscount { get; set; }
    public decimal NetAmount { get; set; }
}