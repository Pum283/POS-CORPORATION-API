namespace Domain;

public class CreateOrUpdateInventoryModel
{
    public Guid StoreId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
}
