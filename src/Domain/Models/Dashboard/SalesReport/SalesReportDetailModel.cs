namespace Domain;

public class SalesReportDetailModel
{
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<SalesReportDetailItemModel> PriceDetails { get; set; } = new();

    // Tổng hợp
    public int TotalQuantitySold => PriceDetails.Sum(x => x.QuantitySold);
    public decimal TotalBeforeDiscount => PriceDetails.Sum(x => x.TotalBeforeDiscount);
    public decimal TotalDiscount => PriceDetails.Sum(x => x.TotalDiscount);
    public decimal NetAmount => TotalBeforeDiscount - TotalDiscount;
}