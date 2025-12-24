
namespace Domain;
public class FoodAppPriceModel
{
    public Guid Id { get; set; }
    public Guid FoodAppPlatformId { get; set; }
    public string PlatformName { get; set; } = default!;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public string SKU { get; set; } = default!;
    public decimal OriginalPrice { get; set; }
    public decimal FoodAppPrice { get; set; }
    public bool IsActive { get; set; }
}
