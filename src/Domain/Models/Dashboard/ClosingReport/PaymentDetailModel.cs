namespace Domain;

public class PaymentDetailModel
{
    public string Method { get; set; } = string.Empty; // e.g., "Tiền mặt", "Chuyển khoản"
    public decimal Amount { get; set; }
}
