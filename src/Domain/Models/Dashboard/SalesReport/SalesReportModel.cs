namespace Domain;

public class SalesReportModel
{
    public SalesReportSummaryModel Summary { get; set; } = new SalesReportSummaryModel();
    public QueryResult<SalesReportDetailModel> Products { get; set; } = new QueryResult<SalesReportDetailModel>();
}