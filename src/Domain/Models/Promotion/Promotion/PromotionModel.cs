namespace Domain;

public class PromotionModel
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public decimal Value { get; set; }
    public string StartDate { get; set; } = default!;
    public string EndDate { get; set; } = default!;
    public int UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public bool IsActive { get; set; }
    public string? ProductScope { get; set; }
    public string? StoreScope { get; set; }
}
