namespace Domain;

public class PermissionActionModel
{
    public string Action { get; set; } = default!;
    public Guid PermissionId { get; set; }
}
