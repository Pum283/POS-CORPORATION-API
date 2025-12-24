namespace Domain;

public class GetInventoryDetailReportQuery
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Guid? StoreId { get; set; }
    public Guid? ProductGroupId { get; set; }
    public Guid? ProductId { get; set; }
}