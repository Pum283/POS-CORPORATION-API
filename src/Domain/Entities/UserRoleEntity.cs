namespace Domain;

[Table("UserRoles")]
public class UserRoleEntity : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public UserEntity User { get; set; } = null!;
    public RoleEntity Role { get; set; } = null!;
    public static UserRoleEntity Create(Guid id, Guid userId, Guid roleId)
    {
        var userRoles = new UserRoleEntity
        {
            Id = id,
            UserId = userId,
            RoleId = roleId
        };
        return userRoles;
    }
}
