namespace Domain;

[Table("LoyaltyPointConfigs")]
public class LoyaltyPointConfigEntity : EntityBase<Guid>
{
    [Column(TypeName = "decimal(18,2)")]
    public decimal EarnRate { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal RedeemRate { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid CreatedBy { get; set; }
    [ForeignKey(nameof(CreatedBy))]
    public UserEntity CreatedByUser { get; set; } = null!;   // ← Có FK, có navigation
    //public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}