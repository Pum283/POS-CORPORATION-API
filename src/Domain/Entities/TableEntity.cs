namespace Domain;

[Table("Tables")]
public class TableEntity : EntityBase<Guid>
{
    [Required]
    public Guid StoreId { get; set; }
    public StoreEntity? Store { get; set; }

    [Required]
    [MaxLength(50)]
    public string TableNumber { get; set; } = default!;

    [Required]
    public TableActiveStatus IsActive { get; set; } // Sử dụng Enum
}