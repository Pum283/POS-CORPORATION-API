namespace Domain;

public class TableCreateRequest
{
    [Required]
    public Guid StoreId { get; set; }

    [Required]
    [MaxLength(50)]
    public string TableNumber { get; set; } = default!;

    public TableActiveStatus IsActive { get; set; } = TableActiveStatus.Active;
}