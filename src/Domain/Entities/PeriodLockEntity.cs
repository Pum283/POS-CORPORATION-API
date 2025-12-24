namespace Domain;

[Table("PeriodLocks")]
public class PeriodLockEntity : EntityBase<Guid>
{
    public Guid StoreId { get; set; }
    public StoreEntity? Store { get; set; }
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
    public ShiftType Shift { get; set; }
    public ShiftPeriodStatusType Status { get; set; } = ShiftPeriodStatusType.Started;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
    public decimal TotalSales { get; set; }
    public decimal? PaymentMoney { get; set; }
    public decimal? FoodAppMoney { get; set; }
    public ICollection<PeriodLockNoteEntity> Note { get; set; } = [];
}