namespace Domain;

public class GetPromotionsQuery
{
    public Guid StoreId { get; set; }
    public Guid ProductId { get; set; }
    public string? PromotionType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
