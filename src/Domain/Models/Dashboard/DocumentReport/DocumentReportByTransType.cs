namespace Domain;

public class DocumentReportByTransType
{
    public string? DocumentName { get; set; }
    public string? ProductName { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    public decimal Quantity { get; set; }
    public UnitType Unit { get; set; }
}
