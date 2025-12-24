namespace Contract;

public enum PromotionConditionType
{
    /// <summary>
    /// Điều kiện áp dụng khi tổng giá trị đơn hàng vượt mức tối thiểu.
    /// </summary>
    MinimumPurchase = 0,

    /// <summary>
    /// Điều kiện áp dụng cho số lượng sản phẩm tối thiểu.
    /// </summary>
    MinimumQuantity = 1,

    /// <summary>
    /// Áp dụng cho sản phẩm cụ thể.
    /// </summary>
    SpecificProduct = 2,

    /// <summary>
    /// Áp dụng khi khách hàng thuộc nhóm đặc biệt (VIP, thành viên, v.v.).
    /// </summary>
    CustomerGroup = 3
}
