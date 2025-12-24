namespace Domain;
[Table("Roles")]
public class RoleEntity : EntityBase<Guid>
{
    [MaxLength(AppConstants.MaxLengthName)]
    [Required]
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<UserRoleEntity> UserRoles { get; set; } = [];
    public virtual ICollection<RolePermissionEntity> RolePermissions { get; set; } = [];

    public static RoleEntity Create(Guid id, string name)
    {
        var roleEntity = new RoleEntity
        {
            Id = id,
            Name = name
        };
        return roleEntity;
    }
}
