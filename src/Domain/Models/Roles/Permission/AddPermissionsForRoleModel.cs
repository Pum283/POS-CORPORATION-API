namespace Domain;

public class AddPermissionsForRoleModel
{
    public Guid RoleId { get; set; }
    public List<Guid> PermissionId { get; set; } = new();
}