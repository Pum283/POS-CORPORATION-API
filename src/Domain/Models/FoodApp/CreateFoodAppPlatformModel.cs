// File: Application/CreateFoodAppPlatformModel.cs
namespace Domain;

public class CreateFoodAppPlatformModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = default!;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public ShippingType ShippingType { get; set; } = ShippingType.None;
}