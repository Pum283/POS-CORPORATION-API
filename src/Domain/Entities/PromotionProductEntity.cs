namespace Domain;

[Table("PromotionProducts")]
public class PromotionProductEntity : EntityBase<Guid>
{
    public Guid PromotionId { get; set; }
    public PromotionEntity Promotion { get; set; } = null!;
    public Guid ProductId { get; set; }
    public ProductEntity Product { get; set; } = null!;
}
