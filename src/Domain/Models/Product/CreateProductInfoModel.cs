namespace Domain;

public class CreateProductInfoModel
{
	public string? SKU { get; set; }
	public string Name { get; set; } = null!;
    public string? Description { get; set; }
	public decimal Price { get; set; }
	public string Unit { get; set; } = null!;
	public Guid ProductGroupId { get; set; }
}
