namespace Domain;

public interface IDataTracking
{
    DateTime CreateDate { get; set; }
    DateTime ModifiedDate { get; set; }
}
