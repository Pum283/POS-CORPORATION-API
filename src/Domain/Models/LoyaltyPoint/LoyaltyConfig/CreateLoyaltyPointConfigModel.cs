// File: Application/CreateLoyaltyPointConfigModel.cs
namespace Domain;

public class CreateLoyaltyPointConfigModel
{
    [Required, Range(0.0001, 99999.99)]
    public decimal EarnRate { get; set; }

    [Required, Range(0.01, 99999.99)]
    public decimal RedeemRate { get; set; }
}