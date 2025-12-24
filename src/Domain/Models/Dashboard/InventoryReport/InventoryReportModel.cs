namespace Domain;

public class InventoryReportModel
{
    //public InventoryReportSummaryModel? Summary { get; set; } 
  
    public QueryResult<ProductInventorySummaryModel>? Products { get; set; }
}
