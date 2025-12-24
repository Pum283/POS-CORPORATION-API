namespace Domain;

public class PeriodLockNoteModel
{
    [JsonIgnore]
    public Guid PeriodLockId { get; set; }
    public decimal SpendingAmount { get; set; }
    public string? Note { get; set; }
}