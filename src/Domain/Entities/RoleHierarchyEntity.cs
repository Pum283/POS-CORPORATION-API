namespace Domain;

[Table("RoleHierarchies")]
public class RoleHierarchyEntity : EntityBase<Guid>
{
    public Guid ParentRoleId { get; set; }
    public RoleEntity? ParentRole { get; set; }

    public Guid ChildRoleId { get; set; }
    public RoleEntity? ChildRole { get; set; }
}
