namespace Domain;

[Table("StoreAccountants")]
public class StoreAccountantEntity : EntityBase<Guid>
{
    public Guid StoreId { get; set; }
    public Guid AccountantId { get; set; }

    public StoreEntity Store { get; set; } = null!;
    public UserEntity Accountant { get; set; } = null!;
    public static StoreAccountantEntity Create(Guid id, Guid storeId, Guid accountantId)
    {
        var storeAccountantEntity = new StoreAccountantEntity
        {
            Id = id,
            StoreId = storeId,
            AccountantId = accountantId
        };
        return storeAccountantEntity;
    }
}
