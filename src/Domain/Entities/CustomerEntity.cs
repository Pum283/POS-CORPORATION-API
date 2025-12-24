namespace Domain;

[Table("Customers")]
public class CustomerEntity : EntityBase<Guid>
{
    [Required]
    public string Name { get; set; } = default!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
}
