namespace Domain;

public class TableResponse
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string TableNumber { get; set; } = default!;
    public TableActiveStatus IsActive { get; set; }
}