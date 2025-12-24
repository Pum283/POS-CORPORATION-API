namespace Domain;

public class Ingredient
{
    public Guid Id { get; set; }
    public string? Code { get; set; } 
    public string? Name { get; set; } 
    public string? Unit { get; set; } 
    public decimal Quantity { get; set; }
}