namespace Domain;

public class PromotionOfProductModel
{
    public string PromotionCategory { get; set; } = null!;
    public List<PromotionDetailOfProductModel> PromotionDetailOfProductModels { get; set; } = [];
}