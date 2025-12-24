namespace Domain;

public class StaffOrdersTableItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid StoreId { get; set; }
    public Guid StaffId { get; set; }
}