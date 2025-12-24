namespace Domain;

public abstract class EntityBase<T> : IEntityBase<T>
{
    [Key]
    public required T Id { get; set; }
    public DateTime CreateDate { get ; set ; }
    public DateTime ModifiedDate { get ; set ; }
}
