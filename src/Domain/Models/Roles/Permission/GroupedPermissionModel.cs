namespace Domain;

public class GroupedPermissionModel
{
    public string Resource { get; set; } = default!;
    public List<PermissionActionModel> Actions { get; set; } = new();
}
