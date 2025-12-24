namespace Domain;

public class DocumentSupplierModel
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string? StoreName { get; set; } = null;
    public Guid? ToStoreId { get; set; } = null;
    public string? ToStoreName { get; set; } = null;
    public Document? Document { get; set; }
    public string? TransType { get; set; }
    public string? Description { get; set; }
    public List<Product> Products { get; set; } = [];
    public List<Ingredient> Ingredients { get; set; } = [];
}
