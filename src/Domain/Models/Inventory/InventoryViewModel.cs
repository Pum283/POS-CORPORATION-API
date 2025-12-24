namespace Domain;

public class InventoryViewModel
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string? StoreName { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal Quantity { get; set; }
}
