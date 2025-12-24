namespace Domain;

public class PromotionStoreModel
{
    public Guid StoreId { get; set; }
    public string StoreName { get; set;} = null!;
    public QueryResult<PromotionModel> Promotions { get; set; } = null!;
}
