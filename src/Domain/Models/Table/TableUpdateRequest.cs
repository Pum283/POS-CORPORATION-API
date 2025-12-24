namespace Domain;

public class TableUpdateRequest
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string TableNumber { get; set; } = default!;

    public TableActiveStatus IsActive { get; set; } = TableActiveStatus.Active;
}