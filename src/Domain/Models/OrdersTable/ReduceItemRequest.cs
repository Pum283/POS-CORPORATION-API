namespace Domain;

public class ReduceItemRequest
{
    public required Guid TableId { get; set; }
    public required string ProductId { get; set; }
    [Range(1, 1000, ErrorMessage = "Số lượng giảm phải từ 1 đến 1000")]
    public int QuantityToReduce { get; set; }
}

public class ReduceResult
{
    public int RemainingQuantity { get; set; }
    public bool WasRemoved { get; set; }
}