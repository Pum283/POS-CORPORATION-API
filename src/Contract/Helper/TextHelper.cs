using System.Globalization;

namespace Contract;

public static class TextHelper
{
    //public static string GenerateOrderCode(string input)
    //{
    //    if (string.IsNullOrWhiteSpace(input))
    //        return string.Empty;

    //    var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
    //    var sb = new StringBuilder();

    //    foreach (var part in parts)
    //    {
    //        if (Regex.IsMatch(part, @"^\d+$"))
    //        {
    //            sb.Append(part); 
    //        }
    //        else
    //        {
    //            sb.Append(char.ToUpper(part[0])); 
    //        }
    //    }

    //    string dateSuffix = DateTime.Now.ToString("ddMMyy");
    //    return $"{sb}-{dateSuffix}";
    //}

    /// <summary>
    /// Tạo mã đơn hàng theo định dạng: OD-hh:mm:ss-dd:MM:yy-StoreName
    /// Ví dụ: OD-14:35:27-09:12:25-CafeHighlands
    /// </summary>
    /// <param name="storeName">Tên cửa hàng (có thể có khoảng trắng)</param>
    /// <returns>Mã đơn hàng</returns>
    public static string GenerateOrderCode(string storeName)
    {
        if (string.IsNullOrWhiteSpace(storeName))
            throw new ArgumentException("StoreName không được để trống.", nameof(storeName));

        var now = DateTime.Now;

        // Lấy giờ:phút:giây
        string timePart = now.ToString("HH:mm:ss");        // 14:35:27

        // Lấy ngày/tháng/năm (2 chữ số)
        string datePart = now.ToString("dd:MM:yy");        // 09:12:25

        // Chuẩn hóa tên cửa hàng: bỏ dấu cách, chỉ lấy chữ cái và số, viết hoa hết
        string cleanStoreName = Regex.Replace(storeName.Trim(), @"[^a-zA-Z0-9]", "").ToUpper();
        // Nếu muốn giữ nguyên tên có dấu thì dùng: storeName.Trim().Replace(" ", "")

        return $"OD-{timePart}-{datePart}-{cleanStoreName}";
    }
    public static string GenerateDocumentName(string transType, string storeName)
    {
        var parts = transType.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var sb = new StringBuilder();

        foreach (var part in parts)
        {
            if (Regex.IsMatch(part, @"^\d+$"))
            {
                sb.Append(part);
            }
            else
            {
                sb.Append(char.ToUpper(part[0]));
            }
        }
        string caLam = DateTime.Now.TimeOfDay < new TimeSpan(14, 30, 0) ? "Sang" : "Chieu";
        string dateSuffix = DateTime.Now.ToString("ddMMyy");
        
        return $"{sb}-{caLam}-{dateSuffix}-{storeName.Replace(" ", "")}";
    }
    public static string GenerateCodePromotion(int length)
    {
        if (length <= 0)
            return string.Empty;

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();

        // 1. Lấy chuỗi GUID (unique) và viết hoa
        var guidPart = Guid.NewGuid().ToString("N").ToUpperInvariant();

        // 2. Lấy chuỗi random
        var randomPart = new string(Enumerable.Range(0, length)
            .Select(_ => chars[random.Next(chars.Length)]).ToArray());

        // 3. Kết hợp GUID và random string
        var combined = guidPart + randomPart;

        // 4. Lấy substring đúng độ dài yêu cầu
        return combined[..Math.Min(length, combined.Length)];
    }

    public static string RemoveDiacritics(string input)
    {
        var normalized = input.Normalize(NormalizationForm.FormD);
        var chars = normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
        return new string(chars.ToArray()).Normalize(NormalizationForm.FormC);
    }
    public static string RemoverUnwrap(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        if (input.StartsWith("\"") && input.EndsWith("\""))
            return input.Substring(1, input.Length - 2).Replace("\\\"", "\"");

        return input;
    }
    public static string GetPrefix(string productGroupName)
    {
        if (string.IsNullOrWhiteSpace(productGroupName))
            return string.Empty;

        // Tách từ theo khoảng trắng
        var words = productGroupName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Lấy ký tự đầu tiên của từng từ, ghép lại, upper-case
        var prefix = string.Concat(words.Select(w => char.ToUpperInvariant(w[0])));

        return prefix;
    }

}
