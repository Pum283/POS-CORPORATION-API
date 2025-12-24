// File: Application/LoyaltyPointConfigModel.cs
namespace Domain;

public class LoyaltyPointConfigModel
{
    public Guid Id { get; set; }
    public decimal EarnRate { get; set; }
    public decimal RedeemRate { get; set; }
    public bool IsActive { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
}