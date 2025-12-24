namespace Domain;

public class SupplierEntity : EntityBase<Guid>
{
    public string? Name { get; set; }
    public string? ContactName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}
