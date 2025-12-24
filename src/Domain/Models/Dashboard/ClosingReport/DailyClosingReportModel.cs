namespace Domain;

public class DailyClosingReportModel
{
    public DailyClosingSummaryModel? Summary { get; set; }
    public QueryResult<DailyProductDetailModel>? Products { get; set; }
}
