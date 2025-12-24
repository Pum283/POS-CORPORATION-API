namespace Domain;

public class ProductCategory
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? CategoryGroup { get; set; }
    public string? AllowedRole { get; set; }

}
