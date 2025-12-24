namespace Domain;

public class User : EntityBase<Guid>
{
    public Guid StoreId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Picture { get; set; }
    public string Password { get; set; } = string.Empty;
    public UserStatusType Status { get; set; }
    public string? Gender { get; set; }
    public string? Phone { get;set; }
    public string Role { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryTime { get; set; }
}
