namespace Domain;

public class UpdateOrderStatusModel
{
    [JsonIgnore]
    public Guid StoreId { get; set; }
    public Guid OrderId { get; set; }
    public string? OrderStatus { get; set; }
    public bool IsUsingPoints { get; set; }
}
