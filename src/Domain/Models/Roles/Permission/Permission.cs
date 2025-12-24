namespace Domain;

public class Permission
{
	public Guid Id { get; set; }
	public string? Resource { get; set; }
	public string? Action { get; set; }
    public override string ToString()
    {
        return $"{Resource}:{Action}";
    }
}
