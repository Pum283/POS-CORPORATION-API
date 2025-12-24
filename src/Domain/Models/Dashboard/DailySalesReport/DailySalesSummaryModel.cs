
namespace Domain;
public class DailySalesSummaryModel
{
    public int TotalQuantitySold { get; set; }
    public decimal TotalOriginalAmount { get; set; } // Tiền trước giảm giá
    public decimal TotalDiscountAmount { get; set; }
    public decimal TotalNetRevenue { get; set; } // Tiền sau giảm giá
}