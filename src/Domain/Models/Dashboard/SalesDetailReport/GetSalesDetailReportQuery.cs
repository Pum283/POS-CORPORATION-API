namespace Domain;

public class GetSalesDetailReportQuery
{
    public Guid? StoreId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public ShippingType? ShippingType { get; set; }
    public Guid? ProductGroupId { get; set; }
}