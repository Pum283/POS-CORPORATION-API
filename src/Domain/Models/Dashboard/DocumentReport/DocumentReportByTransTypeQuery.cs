namespace Domain;

public class DocumentReportByTransTypeQuery
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? TransType { get; set; }
}
