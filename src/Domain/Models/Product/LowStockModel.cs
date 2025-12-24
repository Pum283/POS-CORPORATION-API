namespace Domain;

public class LowStockModel
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = null!;
    public decimal Price { get; set; }
    public string Status { get; set; } = null!;
}
