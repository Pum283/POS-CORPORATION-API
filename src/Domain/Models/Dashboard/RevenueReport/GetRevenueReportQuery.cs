namespace Domain;

public class GetRevenueReportQuery
{
    public Guid? StoreId { get; set; }
    public Guid? ProductGroupId { get; set; }
    public Guid? ProductId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
