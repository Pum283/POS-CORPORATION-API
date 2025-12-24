namespace Domain;

public class DailyClosingSummaryModel
{
    public decimal TotalRevenue { get; set; }
    public int TotalQuantity { get; set; }    
    public int TotalTransactions { get; set; }  
    public DateTime ReportDate { get; set; }
}
