namespace Domain;

public class SalesDetailReportItemModel
{
    public DateTime DateTime { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal BeforeDiscount { get; set; }
    public decimal Discount { get; set; }
    public decimal? Commission { get; set; } // Mặc định null, để mở rộng
    public decimal? ServiceFee { get; set; } // Mặc định null, để mở rộng
    public decimal? Tax { get; set; } // Mặc định null, để mở rộng
    public decimal Revenue { get; set; }
}