namespace Domain;

public class UpdateItemRequest
{
    public Guid TableId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }        // >= 1
    public string? Note { get; set; }
}
