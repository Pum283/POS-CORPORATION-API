namespace Domain;

public class CreateOrUpdateOperationMetricModel
{
    public Guid StoreId { get; set; }
    public string AppType { get; set; } = default!;
    public decimal GMV { get; set; }
    public decimal NetSale { get; set; }
    public int OrderCount { get; set; }
    public DateTime RecordDate { get; set; }
}
