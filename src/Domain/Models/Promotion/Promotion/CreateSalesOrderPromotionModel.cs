namespace Domain;

public class CreateSalesOrderPromotionModel
{
    [JsonIgnore]
    public Guid SalesOrderId { get; set; }
    public Guid PromotionId { get; set; }
}