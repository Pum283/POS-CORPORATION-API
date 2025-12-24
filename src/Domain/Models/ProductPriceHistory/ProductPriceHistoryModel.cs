namespace Domain;

public class ProductPriceHistoryModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public Guid ChangedByUserId { get; set; }
    public string? ChangedByUserName { get; set; } = string.Empty;
    public Guid? StoreId { get; set; }
    public string? StoreName { get; set; }
    public DateTime ChangedAt { get; set; }
}