namespace Domain;

public class CreateOrUpdateStoreAccountantModel
{
    public Guid AccountantId { get; set; }
    public List<Guid> StoreId { get; set; } = [];
}
