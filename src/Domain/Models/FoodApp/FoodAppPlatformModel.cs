
namespace Domain;

public class FoodAppPlatformModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public ShippingType ShippingType { get; set; }
    public string ShippingTypeName => ShippingType.ToString();
    public bool IsActive { get; set; }
    public DateTime CreateDate { get; set; }
}
