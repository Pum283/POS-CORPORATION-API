namespace Domain;

public class MainDiagramOrderDetailModel
{
    public string OrderName { get; set; } = string.Empty; // "Đơn 1", "Đơn 4", v.v.

    public string Type { get; set; } = string.Empty; // "Bán mang về" hoặc "Grab"

    public string? Time { get; set; } // 08:30, 11:20...

    public decimal Amount { get; set; } // Ví dụ: 250000
}
