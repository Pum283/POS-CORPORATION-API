namespace Domain;

public class Promotion
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public PromotionType? Type { get; set; }
    public decimal Value { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public bool IsActive { get; set; }
    public List<string> ProductScope { get; set; } = new();
    public List<string> ProductGroupScope { get; set; } = new();
    public List<string> StoreScope { get; set; } = new();
}
