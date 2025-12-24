namespace Domain;

[Table("PaymentTransactions")]
public class PaymentTransactionEntity : EntityBase<Guid>
{
    public Guid SalesOrderId { get; set; }
    public SalesOrderEntity? SalesOrder { get; set; }
    public string Method { get; set; } = default!;
    public decimal Amount { get; set; }
    public string? Status { get; set; } // Trạng thái: Success, Failed...

}
