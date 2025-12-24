namespace Domain;

public record CreateDocumentSupplierModel
{
    public Guid StoreId { get; set; }
    public string TransType { get; set; } = default!;
    public CreateDocumentModel Document { get; set; } = new();
    public List<AddProductOrIngredientInventoryModel> Products { get; set; } = new();
}