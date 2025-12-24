
namespace Domain;
//public class GetDailySalesReportQuery
//{
//    public Guid StoreId { get; set; }
//    public DateTime FromDate { get; set; }
//    public DateTime ToDate { get; set; }
//}

public class GetDailySalesReportQuery
{
    [Required] public DateTime FromDate { get; set; }
    [Required] public DateTime ToDate { get; set; }
    [Required] public Guid StoreId { get; set; }

    public Guid? ProductGroupId { get; set; }
    public Guid? ProductId { get; set; }
    public ShippingType? ShippingType { get; set; }
    public PaymentMethodType? PaymentMethod { get; set; }
}