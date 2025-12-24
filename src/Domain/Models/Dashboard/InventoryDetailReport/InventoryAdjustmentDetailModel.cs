namespace Domain;

public class InventoryAdjustmentDetailModel
{
    public decimal Quantity { get; set; }
    public DateTime Date { get; set; }
    public string DocumentCode { get; set; } = string.Empty;
    public string? Note { get; set; }
}