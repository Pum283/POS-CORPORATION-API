// File: Domain/Entities/FoodAppPriceEntity.cs
namespace Domain;

[Table("FoodAppPrices")]
public class FoodAppPriceEntity : EntityBase<Guid>
{
    public Guid FoodAppPlatformId { get; set; }
    public FoodAppPlatformEntity FoodAppPlatform { get; set; } = default!;

    public Guid ProductId { get; set; }
    public ProductEntity Product { get; set; } = default!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal OriginalPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal FoodAppPrice { get; set; }

    public bool IsActive { get; set; } = true; // Thêm dòng này
}