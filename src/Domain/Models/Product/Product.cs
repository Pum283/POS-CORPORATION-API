namespace Domain;

public class Product
{
    public Guid Id { get; set; }
    public ProductGroupBasicModel? ProductGroup { get; set; }
    public string? SKU { get; set; }
    public string? Name { get; set; }
    public decimal Quantity { get; set; }
    public string? Picture { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal Cost { get; set; }

    public decimal SalePrice { get; set; }
    public DateTime CreateDate { get; set; }
    public bool IsActive { get; set; }
    public string? Unit { get; set; }
    public decimal? VAT { get; set; }
}
