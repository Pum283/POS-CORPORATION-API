namespace Domain;

public class CreateProductModel
{
    public string? Name { get; set; }
    public string? SKU { get; set; }
    public IFormFile? Picture { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal Cost { get; set; }
    public string? Unit { get; set; }
    public decimal? VAT { get; set; }
    public Guid ProductGroupId { get; set; }
}
