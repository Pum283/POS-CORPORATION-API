namespace Domain;

[Table("SalesOrders")]
public class SalesOrderEntity : EntityBase<Guid>
{
    [Required]
    public string OrderCode { get; set; } = default!;
    public Guid? CustomerId { get; set; }
    public CustomerEntity? Customer { get; set; }
    public Guid StoreId { get; set; }
    public StoreEntity? Store { get; set; }
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    [MaxLength(AppConstants.MaxLengthName)]
    public PaymentMethodType? PaymentMethod { get; set; }
    [MaxLength(AppConstants.MaxLengthName)]
    public OrderStatusType OrderStatus { get; set; }
    [MaxLength(AppConstants.MaxLengthName)]
    public ShippingType ShippingType { get; set; }
    public decimal PaidAmount { get; set; }
    public ICollection<SalesOrderItemEntity>? SalesOrderItem { get; set; }
    public ICollection<SalesOrderPromotionEntity>? SalesOrderPromotion { get; set; }
}
