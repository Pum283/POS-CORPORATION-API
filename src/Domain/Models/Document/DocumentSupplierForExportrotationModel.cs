namespace Domain;

public class DocumentSupplierForExportrotationModel
{
    public Guid FromStoreId { get; set; }
    public string FromStoreName { get; set; } = default!;
    public Guid ToStoreId { get; set; }
    public Guid DocumentId { get; set; }
    public string Description { get; set; } = default!;
    public string? DocumentStatusType { get; set; } 
}
