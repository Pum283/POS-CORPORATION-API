namespace Domain;

[Table("ProductCategories")]
public class ProductCategoryEntity : EntityBase<Guid>
{
    [MaxLength(AppConstants.MaxLengthName)]
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<ProductGroupEntity>? ProductGroup { get; set; }

    public static ProductCategoryEntity Create(Guid id, string name)
    {
        var productCategoryEntity = new ProductCategoryEntity
        {
            Id = id,
            Name = name
        };
        return productCategoryEntity;
    }
}