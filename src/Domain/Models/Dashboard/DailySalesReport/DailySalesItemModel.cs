
namespace Domain;
public class DailySalesItemModel
{
    public ShippingType ShippingType { get; set; }
    public PaymentMethodType PaymentMethod { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal OriginalAmount { get; set; } // Quantity * UnitPrice
    public decimal DiscountAmount { get; set; }
    public decimal NetRevenue { get; set; } // OriginalAmount - DiscountAmount
    public int OrderCount { get; set; } // Số đơn hàng
}