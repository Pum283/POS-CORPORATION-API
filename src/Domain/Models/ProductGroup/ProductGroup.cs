namespace Domain;

public class ProductGroup
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid ProductCategoryId { get; set; }
}
