namespace Domain;

public class UpdatePromotionModel
{
    //public string? Code { get; set; }
    public string? Name { get; set; } 
    public string? PromotionType { get; set; } 
    public decimal Value { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int UsageLimit { get; set; }
    //public int UsageCount { get; set; }
    public bool IsActive { get; set; }
    //public string? ProductScope { get; set; }
    //public string? StoreScope { get; set; }
}
