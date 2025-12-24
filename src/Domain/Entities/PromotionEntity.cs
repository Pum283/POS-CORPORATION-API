namespace Domain;

[Table("Promotions")]
public class PromotionEntity : EntityBase<Guid>
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public PromotionType Type { get; set; } = default!;
    public decimal Value { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public bool IsActive { get; set; }
    public ProductTargetScopeType ProductScope { get; set; } = ProductTargetScopeType.ByProduct;
    public StoreTargetScopeType StoreScope { get; set; } = StoreTargetScopeType.ByStore;

    public ICollection<PromotionConditionEntity> Conditions { get; set; } = [];
    public ICollection<PromotionProductEntity> PromotionProducts { get; set; } = [];
    public ICollection<PromotionProductGroupEntity> PromotionProductGroups { get; set; } = [];
    public ICollection<PromotionStoreEntity> PromotionStores { get; set; } = [];
}
