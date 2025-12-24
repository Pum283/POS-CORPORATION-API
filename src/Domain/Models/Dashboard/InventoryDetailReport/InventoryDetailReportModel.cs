namespace Domain;

public class InventoryDetailReportModel
{
    public QueryResult<InventoryDetailReportItemModel> Products { get; set; } = new();
}