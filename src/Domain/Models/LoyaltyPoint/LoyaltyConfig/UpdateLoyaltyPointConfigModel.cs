// File: Application/UpdateLoyaltyPointConfigModel.cs
namespace Domain;

public class UpdateLoyaltyPointConfigModel
{
    [Range(0.0001, 99999.99)]
    public decimal? EarnRate { get; set; }

    [Range(0.01, 99999.99)]
    public decimal? RedeemRate { get; set; }
}