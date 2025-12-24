namespace Domain;

public class PromotionDetailOfProductModel
{
    public Guid PromotionId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public PromotionType Type { get; set; }
    public decimal Value { get; set; }
    public decimal DiscountAmount { get; set; }
}
