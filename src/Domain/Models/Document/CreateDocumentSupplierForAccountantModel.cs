namespace Domain;

public class CreateDocumentSupplierForAccountantModel
{
    public Guid StoreId { get; set; }
    public string? TransType { get; set; }
    public CreateDocumentModel Document { get; set; } = new();
    public List<AddProductOrIngredientInventoryModel> Products { get; set; } = new();
}
