namespace Domain;

[Table("PromotionStores")]
public class PromotionStoreEntity : EntityBase<Guid>
{
    public Guid PromotionId { get; set; }
    public PromotionEntity Promotion { get; set; } = null!;
    public Guid StoreId { get; set; }
    public StoreEntity Store { get; set; } = null!;
}
