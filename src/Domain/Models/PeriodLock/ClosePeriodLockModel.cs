namespace Domain;

public class ClosePeriodLockModel
{
    [JsonIgnore]
    public Guid StoreId { get; set; }
    [JsonIgnore]
    public Guid UserId { get; set; }
    [JsonIgnore]
    public DateTime EndTime { get; set; } = DateTime.Now;

    public decimal ClosingBalance { get; set; }
    public decimal TotalSales { get; set; }
    public decimal PaymentMoney { get; set; }
    public decimal FoodAppMoney { get; set; }
    public List<PeriodLockNoteModel> Note { get; set; } = [];
}
