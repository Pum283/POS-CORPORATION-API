namespace Domain;

public class CreateUserModel
{
    public Guid? StoreId { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? RepeatPassword { get; set; }
    public string? Role { get; set; }
}
