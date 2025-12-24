namespace Domain;

public class RevenueReportSummaryModel
{
    public decimal TotalRevenue { get; set; }

    public int TotalOrders { get; set; }

    public int TotalStores { get; set; }

    public decimal AverageRevenuePerStore { get; set; }
}
