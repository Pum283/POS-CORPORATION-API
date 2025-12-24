using System.ComponentModel;

namespace Contract;

public enum TransType
{
    [Description("Nhập hàng")]
    GoodsReceipt = 1,

    [Description("Xuất hoàn kho tổng")]
    ReturnedGoodsReceipt = 2,

    [Description("Nhập điều chỉnh")]
    AdjustmentEntry = 3,

    [Description("Xuất trả lại")]
    ReturnOutput = 4,

    [Description("Hủy sản phẩm")]
    CanceledProduct = 5,

    [Description("Xuất luân chuyển")]
    Exportrotation = 6
}
