namespace Domain;

public class DailyProductDetailModel
{
    public string ProductName { get; set; } = string.Empty;
    public string ProductGroup { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }

    public List<PaymentDetailModel> PaymentMethods { get; set; } = new();
}
