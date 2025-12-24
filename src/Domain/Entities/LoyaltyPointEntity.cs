namespace Domain;

[Table("LoyaltyPoints")]
public class LoyaltyPointEntity : EntityBase<Guid>
{
    public Guid CustomerId { get; set; }
    public int Points { get; set; }
}
