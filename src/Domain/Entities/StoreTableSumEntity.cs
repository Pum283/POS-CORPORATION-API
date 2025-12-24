namespace Domain;

[Table("StoreTableSums")]
public class StoreTableSumEntity : EntityBase<Guid>
{
    [Required]
    public Guid StoreId { get; set; }
    public StoreEntity? Store { get; set; }

    [Required]
    public int TableCount { get; set; }
}