namespace Domain;

public class InventoryDetailReportItemModel
{
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;

    public decimal OpeningQuantity { get; set; }   // Tồn đầu kỳ
    public decimal ClosingQuantity { get; set; }   // Tồn cuối kỳ
    public decimal SoldQuantity { get; set; }      // Bán ra (tính toán)

    public decimal TotalInput { get; set; }
    public decimal TotalOutput { get; set; }

    // Phân loại chi tiết Nhập/Xuất (chỉ tổng theo loại)
    public InventoryInputBreakdownModel InputBreakdown { get; set; } = new();
    public InventoryOutputBreakdownModel OutputBreakdown { get; set; } = new();

    // Chỉ phần này cần list chi tiết
    public List<InventoryAdjustmentDetailModel> Adjustments { get; set; } = new();
}