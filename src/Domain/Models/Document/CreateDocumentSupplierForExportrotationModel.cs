namespace Domain;

public class CreateDocumentSupplierForExportrotationModel
{
    [JsonIgnore]
    public Guid FromStoreId { get; set; }
    public Guid ToStoreId { get; set; }
    public CreateDocumentModel Document { get; set; } = new();
    public List<AddProductOrIngredientInventoryWhenDocumentExportrotationModel> Products { get; set; } = new();
}
