namespace Domain;

public class SalesDetailReportModel
{
    public SalesDetailReportSummaryModel Summary { get; set; } = new SalesDetailReportSummaryModel();
    public QueryResult<SalesDetailReportItemModel> Details { get; set; } = new QueryResult<SalesDetailReportItemModel>();
}