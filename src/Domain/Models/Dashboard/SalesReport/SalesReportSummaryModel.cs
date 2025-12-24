namespace Domain;

public class SalesReportSummaryModel
{
    public int TotalQuantitySold { get; set; }
    public decimal TotalOriginalAmount { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalNetAmount { get; set; }
}