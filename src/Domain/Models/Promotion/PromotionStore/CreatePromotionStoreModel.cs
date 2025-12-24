namespace Domain;

public class CreatePromotionStoreModel
{
    public Guid? StoreId { get; set; } 
    public Guid? ProductId { get; set; } 
    public Guid? ProductGroupId { get; set; } 
    public CreatePromotionModel CreatePromotionModel { get; set; } = null!;
}
