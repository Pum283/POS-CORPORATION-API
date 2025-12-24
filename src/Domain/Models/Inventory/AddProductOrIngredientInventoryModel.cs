namespace Domain;

public record AddProductOrIngredientInventoryModel
{
    public string Code {  get; set; } = default!;
    public decimal Quantity { get; set; }
}
