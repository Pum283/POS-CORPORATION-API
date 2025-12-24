namespace Domain;

public class DocumentForExportrotationQuery
{
    public string DocumentStatusType { get; set; } = default!;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? IngredientId { get; set; }
}
