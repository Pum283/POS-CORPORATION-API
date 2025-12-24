namespace Domain;

[Table("Banks")]
public class BankEntity : EntityBase<Guid>
{
    public Guid AccountId { get; set; }
    public long accountNo { get; set; }
    public string accountName { get; set; } = string.Empty;
    public int acqId { get; set; }
    public string format { get; set; } = "text";
    public string template { get; set; } = "compact";
}
