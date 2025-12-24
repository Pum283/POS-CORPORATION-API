namespace Domain;

public class TopStoreRevenueModel
{
    public Guid StoreId { get; set; }
    public string StoreName { get; set; } = null!;
    public int TotalOrder { get; set; }
    public decimal TotalRevenue { get; set; }
}
