namespace Domain;

public class CreateSalesOrderItemModel
{
    [JsonIgnore]
    public Guid SalesOrderId { get; set; }
    [JsonIgnore]
    public decimal UnitPrice { get; set; }
    [JsonIgnore]
    public decimal TotalPrice { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public Guid? PromotionId { get; set; }

}
