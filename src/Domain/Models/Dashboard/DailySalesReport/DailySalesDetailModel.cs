
namespace Domain;
public class DailySalesDetailModel
{
    public Guid ProductId { get; set; }
    public string ProductSku { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ProductGroupName { get; set; } = string.Empty;
    public List<DailySalesItemModel> Items { get; set; } = new List<DailySalesItemModel>();
    public int TotalQuantity { get; set; }
    public decimal TotalOriginalAmount { get; set; }
    public decimal TotalDiscountAmount { get; set; }
    public decimal TotalNetRevenue { get; set; }
}