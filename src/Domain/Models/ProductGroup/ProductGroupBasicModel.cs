namespace Domain;

public record ProductGroupBasicModel
{
    public Guid Id { get; set; } 
    public string Name { get; set; } = string.Empty;
}
