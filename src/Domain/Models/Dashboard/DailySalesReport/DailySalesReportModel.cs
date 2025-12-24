
namespace Domain;
//public class DailySalesReportModel
//{
//    public DateTime FromDate { get; set; }
//    public DateTime ToDate { get; set; }
//    public Guid StoreId { get; set; }
//    public string StoreName { get; set; } = string.Empty;
//    public DailySalesSummaryModel Summary { get; set; } = new DailySalesSummaryModel();
//    public List<DailySalesDetailModel> Details { get; set; } = new List<DailySalesDetailModel>();
//}
public class DailySalesReportModel
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Guid StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;

    // Các filter hiện tại đang áp dụng (có thể null)
    public string? ProductGroupName { get; set; }
    public string? ProductName { get; set; }
    public string? SKU { get; set; }
    public string? ShippingType { get; set; }
    public string? PaymentMethod { get; set; }

    // Kết quả tổng hợp theo filter
    public int TotalOrders { get; set; }
    public int TotalQuantitySold { get; set; }
    public decimal TotalOriginalAmount { get; set; }
    public decimal TotalDiscountAmount { get; set; }
    public decimal TotalNetRevenue { get; set; }
}