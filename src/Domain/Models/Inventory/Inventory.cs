namespace Domain;

public class Inventory
{
    public Guid Id { get; set; }
    public Store? Store { get; set; }
    public Product? Product { get; set; }
    public decimal Quantity { get; set; }
}
