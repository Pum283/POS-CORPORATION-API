namespace Domain;

[Table("LoyaltyTransactions")]
public class LoyaltyTransactionEntity : EntityBase<Guid>
{
    public Guid CustomerId { get; set; }
    public Guid SalesOrderId { get; set; }
    public int PointsEarned { get; set; }
    public int PointsUsed { get; set; }
}
