namespace Domain;

public class StoreAccountantModel
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string? StoreName { get; set; }
    public Guid AccountantId { get; set; }
}
