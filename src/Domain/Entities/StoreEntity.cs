namespace Domain;

[Table("Stores")]
public class StoreEntity : EntityBase<Guid>
{
    [Required]
    public string Name { get; set; } = default!;
    [Required]
    public string Address { get; set; } = default!;
    [Required]
    public string Phone { get; set; } = default!;
    //public Guid? BankId { get; set; }
    public ICollection<UserEntity>? User { get; set; }
    public ICollection<InventoryEntity>? Inventory { get; set; }
    public ICollection<IngredientInventoryEntity>? IngredientInventory { get; set; }
    public ICollection<SalesOrderEntity>? SalesOrder { get; set; }
    public ICollection<PurchaseOrderEntity>? PurchaseOrder { get; set; }
    public static StoreEntity Create(Guid id, string name, string address, string phone)
    {
        var storeEntity = new StoreEntity
        {
            Id = id,
            Name = name,
            Address = address,
            Phone = phone
        };
        return storeEntity;
    }
}
