namespace Domain;

public class Role
{
	public Guid Id { get; set; }
	public string? Name { get; set; }
    public Guid? ParentRoleId { get; set; }   // Cha của role này
    public Guid? HierarchyId { get; set; }    // Id của dòng trong bảng RoleHierarchies
}
