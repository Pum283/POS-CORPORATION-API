namespace Domain;

[Table("SalesOrderPromotions")]
public class SalesOrderPromotionEntity : EntityBase<Guid>
{
    public Guid SalesOrderId { get; set; }
    public SalesOrderEntity? SalesOrder { get; set; }
    public Guid PromotionId { get; set; }
    public PromotionEntity? Promotion { get; set; }
    public decimal DiscountAmount { get; set; }
    public Guid? SalesOrderItemId { get; set; } = null;                    // Cột mới
    [ForeignKey("SalesOrderItemId")]
    public SalesOrderItemEntity? SalesOrderItem { get; set; }              // Navigation (EF tự map)
}
