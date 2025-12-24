namespace Domain;

[Table("TimeSales")]
public class TimeSaleEntity : EntityBase<Guid>
{
    public PromotionType Type { get; set; } = default!;
    public decimal Value { get; set; }
    public DateTime? StartTime { get; set; } 
    public DateTime? EndTime { get; set; } 
    public bool Status { get; set; }
}