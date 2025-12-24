namespace Domain;

public class SalesOrderPromotion
{
    public Guid Id { get; set; }
    public Guid SalesOrderId { get; set; }
    public SalesOrderItem? SalesOrder { get; set; }
    public Guid PromotionId { get; set; }
    public decimal DiscountAmount { get; set; }
    public Promotion? Promotion { get; set; }
}
