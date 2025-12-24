namespace Domain.Models;

public class CreateDocumentSupplierForTransferModel
{
    public Guid ToStoreId { get; set; }
    public CreateDocumentModel Document { get; set; } = new();
    public List<AddProductOrIngredientInventoryModel> Products { get; set; } = new();
}
