namespace Domain;

public class DashboardQuery
{
    public Guid? StoreId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}
