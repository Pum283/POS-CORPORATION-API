namespace Domain;

[Table("PromotionProductGroups")]
public class PromotionProductGroupEntity : EntityBase<Guid>
{
    public Guid PromotionId { get; set; }
    public PromotionEntity Promotion { get; set; } = null!;
    public Guid ProductGroupId { get; set; }
    public ProductGroupEntity ProductGroup { get; set; } = null!;
}
