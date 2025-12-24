// File: Domain/Entities/ProductPriceHistoryEntity.cs
namespace Domain;

[Table("ProductPriceHistories")]
public class ProductPriceHistoryEntity : EntityBase<Guid>
{
    public Guid ProductId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OldPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal NewPrice { get; set; }

    public Guid ChangedByUserId { get; set; }

    public string? Reason { get; set; } // nullable

    // Factory đúng style bạn (bắt buộc có)
    public static ProductPriceHistoryEntity Create(
        Guid productId,
        decimal oldPrice,
        decimal newPrice,
        Guid changedByUserId,
        string? reason = null)
    {
        return new ProductPriceHistoryEntity
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            OldPrice = oldPrice,
            NewPrice = newPrice,
            ChangedByUserId = changedByUserId,
            Reason = reason?.Trim()
            // CreateDate tự động từ EntityBase
        };
    }
}