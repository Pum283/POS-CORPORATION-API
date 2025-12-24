namespace Domain;

public interface ISoftDelete
{
    public DateTime? DeleteTime { get; set; }
    public bool IsDeleted { get; set; }
}
