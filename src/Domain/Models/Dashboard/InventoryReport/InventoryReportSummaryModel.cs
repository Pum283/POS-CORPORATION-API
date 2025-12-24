namespace Domain;

public class InventoryReportSummaryModel
{
    public int TotalProduct { get; set; }
    public HashSet<InventoryWarningModel>? TotalLowStockWarnings { get; set; }
    public List<TopProductBestSellingModel>? TopProducts { get; set; }
}
