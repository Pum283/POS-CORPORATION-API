namespace Domain;

[Table("RolePermissions")]
public class RolePermissionEntity : EntityBase<Guid>
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public RoleEntity? Role { get; set; }
    public PermissionEntity? Permission { get; set; }
}
