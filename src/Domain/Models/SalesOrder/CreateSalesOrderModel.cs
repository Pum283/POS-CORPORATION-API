namespace Domain;

public class CreateSalesOrderModel
{
    [JsonIgnore]
    public Guid StoreId { get; set; }
    [JsonIgnore]
    public decimal TotalAmount { get; set; }
    [JsonIgnore]
    public decimal Discount { get; set; }
    [JsonIgnore]
    public Guid UserId { get; set; }

    //public List<Guid>? PromotionId { get; set; } = [];
    public Guid? CustomerId { get; set; }
    public bool IsUsingPoints { get; set; }
    public string? PaymentMethod { get; set; }
    public string? ShippingType { get; set; }

    public List<CreateSalesOrderItemModel>? SalesOrderItem { get; set; }
    public List<CreateSalesOrderPromotionModel>? SalesOrderPromotion { get; set; }
}
