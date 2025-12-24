namespace Domain;

public abstract class EntityAuditBase<T> : IEntityBase<T>, IAuditable
{
    public T Id { get; set; } = default!;
    public DateTime CreateDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
