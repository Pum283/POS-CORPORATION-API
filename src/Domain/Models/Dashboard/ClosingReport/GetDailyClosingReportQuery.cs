namespace Domain;

public class GetDailyClosingReportQuery
{
    public DateTime Date { get; set; }
    [JsonIgnore]
    public Guid? StoreId { get; set; } 
}
