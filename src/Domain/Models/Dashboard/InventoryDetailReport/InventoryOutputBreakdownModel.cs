namespace Domain;

public class InventoryOutputBreakdownModel
{
    public decimal ReturnedGoodsReceipt { get; set; } // Xuất hoàn kho tổng
    public decimal CanceledProduct { get; set; }     // Hủy sản phẩm
    public decimal ExportRotation { get; set; }      // Xuất luân chuyển
}
