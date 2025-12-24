namespace Domain;

public class AccountingInventoryModel
{
    //public Guid InventoryId { get; set; }
    //public Guid ProductId { get; set; }
    //public string Code { get; set; } = null!;
    //public string Name { get; set; } = null!;
    //public string Unit { get; set; } = null!;
    //public decimal InitialQuantity { get; set; }    
    //public decimal CurrentQuantity { get; set; }     
    //public decimal RealQuantity { get; set; }
    //public List<AccountingDocumentModel> InputQuantity { get; set; } = [];
    //public List<AccountingDocumentModel> OutputQuantity { get; set; } = [];
    //public bool IsActive { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? InventoryId { get; set; }
    public string? ProductName { get; set; } = "";
    public string? ProductSku { get; set; } = "";
    public string? IngredientName { get; set; } = "";
    public string? IngredientCode { get; set; } = "";
    public decimal InitialQuantity { get; set; }
    public decimal RealQuantity { get; set; }
    public decimal SoldQuantity { get; set; }
    public List<AccountingDocumentModel> InputQuantity { get; set; } = new();
    public List<AccountingDocumentModel> OutputQuantity { get; set; } = new();
    public List<AdjustmentDetailModel> Adjustments { get; set; } = new();
}
