
namespace Domain;

public class AddItemRequest
{
    public required Guid TableId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
}