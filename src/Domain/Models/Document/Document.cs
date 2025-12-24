namespace Domain;

public record Document
{
    public Guid Id { get; set; }
    public string? DocumentName { get; set; }
    public string? Status { get; set; }
    public string? CreateDate { get; set; }
    public Guid UserCreateId { get; set; }
    public string? UserCreateName {get; set;}

}
