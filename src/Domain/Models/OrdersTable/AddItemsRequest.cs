﻿namespace Domain;

public class AddItemsRequest
{
    public required Guid TableId { get; set; }
    public required List<AddItemDetail> Items { get; set; }
}

public class AddItemDetail
{
    [Required(ErrorMessage = "ProductId là bắt buộc")]
    public Guid ProductId { get; init; } = default;

    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
    public int Quantity { get; init; } = 1;
    public string? Note { get; set; }
}