namespace Domain;

public class IngredientsInventoryReportModel
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Unit { get; set; }
    public decimal OriginalQuantity { get; set; }   // Số lượng ban đầu
    public decimal BuyQuantity { get; set; }    // Số lượng đã bán
    public decimal InputQuantity { get; set; } // Số lượng nhập kho
    public decimal Quantity { get; set; } // số lượng trong kho
}
