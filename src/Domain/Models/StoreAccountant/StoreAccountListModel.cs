namespace Domain;

public class StoreAccountListModel
{
    public Guid Id { get; set; }
    public Guid AccountantId { get; set; }
    public List<StoreModel> Stores { get; set; } = [];
}
