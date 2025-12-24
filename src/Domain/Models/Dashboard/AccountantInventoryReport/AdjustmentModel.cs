namespace Domain;

public class AdjustmentDetailModel
{
    public decimal Quantity { get; set; }           // có thể âm (xuất điều chỉnh) hoặc dương (nhập điều chỉnh)
    public DateTime Date { get; set; }              // ngày giờ tạo chứng từ
    public string? DocumentName { get; set; }       // tùy chọn, để dễ đối chiếu
}