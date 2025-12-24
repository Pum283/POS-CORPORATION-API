namespace Domain;

public interface IEntityBase<T> : IDataTracking
{
     T Id { get; set; }
}
