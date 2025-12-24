namespace Domain;

[Table("PeriodLockNotes")]
public class PeriodLockNoteEntity : EntityBase<Guid>
{
    public Guid PeriodLockId { get; set; }
    public PeriodLockEntity? PeriodLock { get; set; }
    public decimal SpendingAmount { get; set; }
    public string Note { get; set; } = string.Empty;
}
