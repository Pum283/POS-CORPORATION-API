namespace Domain;

public class ProductInventorySummaryModel
{
    public string? SKU { get; set; }
    public string? Name { get; set; }
    public string? Unit { get; set; }

    public decimal OriginalQuantity { get; set; }   // Số lượng ban đầu
    public decimal BuyQuantity { get; set; }    // Số lượng đã bán

    public decimal Quantity { get; set; } // số lượng trong kho
    public decimal InputQuantity { get; set; } // Số lượng nhập kho
    public Dictionary<ShippingType, int> SoldByShippingType { get; set; } = new();
}