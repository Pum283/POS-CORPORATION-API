namespace Domain;

public class CreatePeriodLockModel
{
    [JsonIgnore]
    public Guid StoreId { get; set; }
    [JsonIgnore]
    public Guid UserId { get; set; }
    [JsonIgnore]
    public DateTime StartTime { get; set; } = DateTime.Now;

    public string? Shift { get; set; }
    public decimal OpeningBalance { get; set; }

    //public DateTime EndTime { get; set; }
}
