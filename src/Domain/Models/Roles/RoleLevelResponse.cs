namespace Domain;

public class RoleLevelResponse
{
    public int Level { get; set; }
    public List<Role> Roles { get; set; } = new();
}