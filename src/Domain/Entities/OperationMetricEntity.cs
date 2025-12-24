using System.Security.AccessControl;

namespace Domain;

[Table("OperationMetrics")]
public class OperationMetricEntity : EntityBase<Guid>
{
    public Guid StoreId { get; set; }
    public required StoreEntity Store { get; set; }
    public AppType AppType { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal GMV { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal NetSale { get; set; }
    public int OrderCount { get; set; }
    [Column(TypeName = "date")]
    public DateTime RecordDate { get; set; }
}
