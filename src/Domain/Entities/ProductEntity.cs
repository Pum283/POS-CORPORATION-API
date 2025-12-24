namespace Domain;

[Table("Products")]
public class ProductEntity : EntityBase<Guid>
{
    public Guid? ProductGroupId { get; set; }
    public ProductGroupEntity? ProductGroup { get; set; }
    [Required]
    public string SKU { get; set; } = string.Empty;
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Picture { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; } // Giá bán
    public decimal Cost { get; set; } // Giá vốn
    public UnitType Unit { get; set; }
    public decimal? VAT { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<ProductIngredientEntity> ProductIngredients { get; set; } = new List<ProductIngredientEntity>();
    public ICollection<InventoryPeriodLockEntity> InventoryShiftSnapshots { get; set; } = new List<InventoryPeriodLockEntity>();

    public static ProductEntity Create(Guid id, Guid productGroupId, string sku, string name, string description, string picture, decimal price, decimal cost, UnitType unit, bool IsActive)
    {
        var productEntity = new ProductEntity
        {
            Id = id,
            ProductGroupId = productGroupId,
            SKU = sku,
            Name = name,
            Description = description,
            Picture = picture,
            Price = price,
            Cost = cost,
            Unit = unit,
            IsActive = IsActive
        };
        return productEntity;
    }
}
