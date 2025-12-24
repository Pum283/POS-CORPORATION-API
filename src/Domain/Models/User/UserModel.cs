namespace Domain;

public class UserModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Picture { get; set; }
    public string? Gender { get; set; }
    public string? Phone { get; set; }
    public string? DOB { get; set; }
    public string? Address { get; set; }
    public string Role { get; set; } = string.Empty;
    public string? StoreName { get; set; }
}
