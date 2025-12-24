namespace Domain;

public class UpdateUserModel
{
    public string? Name { get; set; }
    public IFormFile? Picture { get; set; }
    public string? Gender { get; set; }
    public string? Phone { get; set; }
    public DateTime DOB { get; set; }
    public string? Address { get; set; }
}
