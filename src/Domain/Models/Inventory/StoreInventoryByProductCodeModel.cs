namespace Domain;

public class StoreInventoryByProductCodeModel
{
    public Guid StoreId { get; set; }
    public string StoreName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string Status { get; set; } = null!;
}
