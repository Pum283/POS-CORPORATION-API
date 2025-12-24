namespace Domain;

[Table("DocumentSuppliers")]
public class DocumentSupplierEntity : EntityBase<Guid>
{
    public Guid StoreId { get; set; }
    public Guid? ToStoreId { get; set; }
    public Guid DocumentId { get; set; }
    public DocumentEntity? Document { get; set; }
    public Guid? SupplierId { get; set; }
    public SupplierEntity? Supplier { get; set; }
    public Guid? ProductId { get; set; }
    public ProductEntity? Product { get; set; }
    public Guid? IngredientId { get; set; }
    public IngredientEntity? Ingredient { get; set; }
    public decimal Quantity { get; set; }
    public TransType TransType { get; set; }
}
