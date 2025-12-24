namespace Domain;

public class LoyaltyPoint
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public int Points { get; set; }
}
