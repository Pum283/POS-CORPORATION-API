namespace Domain;

public class CreateOrUpdateProductGroupModel
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid ProductCategoryId { get; set; }
}
