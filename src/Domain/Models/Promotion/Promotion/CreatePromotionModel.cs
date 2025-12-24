namespace Domain;

public class CreatePromotionModel
{
    [JsonIgnore]
    public Guid PromotionId { get; set; }
    [JsonIgnore]
    public string? Code { get; set; }
    public string Name { get; set; } = default!;
    public decimal Value { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? UsageLimit { get; set; }
    public bool IsActive { get; set; }
    public string? PromotionType { get; set; }
    public string? ProductScope { get; set; }
    public string? StoreScope { get; set; }
}
