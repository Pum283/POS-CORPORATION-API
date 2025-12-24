namespace Domain;

public class InventoryInputBreakdownModel
{
    public decimal GoodsReceipt { get; set; }     // Nhập hàng
    public decimal ReturnOutput { get; set; }     // Xuất trả lại (nhập lại)
    public decimal ImportRotation { get; set; }   // Nhập luân chuyển
}