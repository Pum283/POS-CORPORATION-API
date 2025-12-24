// Domain/Dto/CreateSalesOrderRequest.cs
namespace Domain;

public class CreateSalesOrderRequest
{
    public Guid StoreId { get; set; }
    public Guid? CustomerId { get; set; }
    public bool IsUsingPoints { get; set; } = false;
    public string PaymentMethod { get; set; } = "COD"; // string để dễ parse
    public string ShippingType { get; set; } = "None";

    public List<CreateSalesOrderItemRequest> SalesOrderItem { get; set; } = null!;
}

public class CreateSalesOrderItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public Guid? PromotionId { get; set; } // Đây là field mới – chỉ ở DTO
}