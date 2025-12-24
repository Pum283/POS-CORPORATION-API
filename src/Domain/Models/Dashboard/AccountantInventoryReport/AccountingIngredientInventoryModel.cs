namespace Domain;

public class AccountingIngredientInventoryModel
{
    public Guid IngredientInventoryId { get; set; }
    public Guid IngredientId { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Unit { get; set; }
    public decimal InitialQuantity { get; set; }
    public decimal CurrentQuantity { get; set; }
    public decimal? RealQuantity { get; set; }
    public List<AccountingDocumentModel> InputQuantity { get; set; } = [];
    public List<AccountingDocumentModel> OutputQuantity { get; set; } = [];
}
