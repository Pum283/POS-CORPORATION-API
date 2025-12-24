namespace Domain;

public class MainDiagramReportModel
{
    public MainDiagramReportSummaryModel Summary { get; set; } = new();
    public QueryResult<MainDiagramOrderDetailModel> Orders { get; set; } = new();
}
