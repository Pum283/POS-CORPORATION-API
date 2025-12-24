
namespace Domain;
public class CreateFoodAppPriceModel
{
    [Required] public Guid FoodAppPlatformId { get; set; }
    [Required] public Guid ProductId { get; set; }
    [Required] public decimal FoodAppPrice { get; set; }
}
