namespace Domain;

public class UpdateDocumentStatusModel
{
    public Guid FromStoreId { get; set; }
    public Guid ToStoreId { get; set; }
    public string Status { get; set; } = default!;
}
