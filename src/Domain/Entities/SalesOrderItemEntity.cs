namespace Domain;

public class SalesOrderItemEntity : EntityBase<Guid>
{
    public Guid SalesOrderId { get; set; }
    public SalesOrderEntity? SalesOrder { get; set; }
    public Guid ProductId { get; set; }
    public ProductEntity? Product { get; set; }
    [MaxLength(AppConstants.MaxLengthDescription)]
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

}
