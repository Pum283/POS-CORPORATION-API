namespace Domain;

[Table("Permissions")]
public class PermissionEntity : EntityBase<Guid>
{
    //public PermissionType Type { get; set; }
    [MaxLength(AppConstants.MaxLengthPermision)]
    public string Resource { get; set; } = string.Empty;
    [MaxLength(AppConstants.MaxLengthPermision)]
    public string Action { get; set; } = string.Empty;
    public virtual ICollection<RolePermissionEntity> RolePermissions { get; set; } = [];
    public static PermissionEntity Create(Guid id, string resource, string action)
    {
        return new PermissionEntity
        {
            Id = id,
            Resource = resource,
            Action = action
        };
    }
}
