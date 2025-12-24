namespace Domain;

[Table("PromotionConditions")]
public class PromotionConditionEntity : EntityBase<Guid>
{
    public Guid PromotionId { get; set; }
    public PromotionEntity? Promotion { get; set; }
    public PromotionConditionType? ConditionType { get; set; }
    public string? ConditionValue { get; set; }
}
