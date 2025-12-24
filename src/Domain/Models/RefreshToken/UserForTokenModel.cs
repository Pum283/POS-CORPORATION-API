namespace Domain;

public class UserForTokenModel
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Picture { get; set; }
    public List<Role> Roles { get; set; } = [];
	public List<Permission> Permissions { get; set; } = [];
    public bool IsDeleted { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}
