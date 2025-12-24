namespace Domain;

public class RevenueReportModel
{
    public RevenueReportSummaryModel? Summary { get; set; }

    public QueryResult<RevenueStoreDetailModel>? Stores { get; set; }
}
