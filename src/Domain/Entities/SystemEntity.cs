namespace Domain;

[Table("Systems")]
public class SystemEntity : EntityBase<Guid>
{
    public string? ColorSystem { get; set; }
    public string? TextSize { get; set; }
}
