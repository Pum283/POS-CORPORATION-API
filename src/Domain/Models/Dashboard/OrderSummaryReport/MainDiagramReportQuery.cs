namespace Domain;

public class MainDiagramReportQuery
{
    [JsonIgnore]
    public Guid StoreId { get; set; }
    public DateTime Date { get; set; }
    public string? ShippingType { get; set; }
}
