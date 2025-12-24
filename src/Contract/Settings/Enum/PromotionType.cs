namespace Contract;

public enum PromotionType
{
    // Giảm giá theo tỷ lệ phần trăm (ví dụ: 10%, 20%)
    Percentage = 1,

    // Giảm giá theo số tiền cố định (ví dụ: 50.000 VNĐ, 10 USD)
    FixedAmount = 2

    //// Giảm giá khi mua một số lượng sản phẩm nhất định
    //Volume = 3,

    //// Giảm giá khi mua nhiều sản phẩm trong một gói/combo
    //Bundle = 4,

    //// Giảm giá dành cho khách hàng thân thiết
    //Loyalty = 5,

    //// Giảm giá vào các dịp đặc biệt, ngày lễ, mùa
    //SeasonalHoliday = 6,

    //// Giảm giá cho lần mua hàng đầu tiên của khách hàng
    //FirstPurchase = 7,

    //// Giảm giá vận chuyển hoặc miễn phí vận chuyển
    //Shipping = 8,

    //// Một loại giảm giá khác không thuộc các loại trên, cần chỉ định rõ
    //Other = 9
}
