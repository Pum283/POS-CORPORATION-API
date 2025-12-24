namespace Domain;

public class AddOrUpdateRoleHierarchieModel
{
    public Guid ParentRoleId { get; set; }
    public Guid ChildRoleId { get; set; }
}
