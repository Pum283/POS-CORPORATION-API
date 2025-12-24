namespace Domain;
[Table("ProductGroups")]
public class ProductGroupEntity : EntityBase<Guid>
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ProductCategoryId { get; set; }
    public ProductCategoryEntity? ProductCategory { get; set; }
    public ICollection<ProductEntity>? Product { get; set; }

    public static ProductGroupEntity Create(Guid id, string name, Guid productCategoryId)
    {
        var productGroupEntity = new ProductGroupEntity
        {
            Id = id,
            Name = name,
            ProductCategoryId = productCategoryId
        };
        return productGroupEntity;
    }
}
