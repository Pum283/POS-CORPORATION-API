// File: Domain/Entities/FoodAppPlatformEntity.cs
namespace Domain;

[Table("FoodAppPlatforms")]
public class FoodAppPlatformEntity : EntityBase<Guid>
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    [MaxLength(500)]
    public string? Description { get; set; }

    public ShippingType ShippingType { get; set; } = ShippingType.None;

    public bool IsActive { get; set; } = true; // Thêm dòng này

    // Navigation
    public ICollection<FoodAppPriceEntity> FoodAppPrices { get; set; } = new List<FoodAppPriceEntity>();
}