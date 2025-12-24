using System.Reflection;
using ClosedXML.Excel;

namespace Application;

public class ClosedXMLExcelExporter<T> : IExcelExporter<T>
{
    public byte[] ExportToExcel(IEnumerable<T> data, string sheetName = "Data")
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add(sheetName);

            var properties = typeof(T).GetProperties();

            // Header
            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = properties[i].Name;
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            }

            // Rows
            int row = 2;
            foreach (var item in data)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    var value = properties[col].GetValue(item);
                    worksheet.Cell(row, col + 1).Value = value != null ? XLCellValue.FromObject(value) : XLCellValue.FromObject("");
                }
                row++;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }

    public byte[] ExportToExcel(Dictionary<string, IEnumerable<T>> dataBySheet)
    {
        using (var workbook = new XLWorkbook())
        {
            var properties = typeof(T).GetProperties();

            foreach (var sheet in dataBySheet)
            {
                var worksheet = workbook.Worksheets.Add(sheet.Key);

                // Header
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = properties[i].Name;
                    worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                }

                // Rows
                int row = 2;
                foreach (var item in sheet.Value)
                {
                    for (int col = 0; col < properties.Length; col++)
                    {
                        var value = properties[col].GetValue(item);
                        worksheet.Cell(row, col + 1).Value = value != null ? XLCellValue.FromObject(value) : XLCellValue.FromObject("");
                    }
                    row++;
                }
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }
    public byte[] ExportInvoice(List<T> data, string store, string creator, string docType, DateTime createdAt)
    {
        using (var workbook = new XLWorkbook())
        {
            var ws = workbook.Worksheets.Add("Invoice");

            // Thông tin header
            ws.Cell("B1").Value = "Cửa hàng:";
            ws.Cell("C1").Value = store;

            ws.Cell("B2").Value = "Người tạo:";
            ws.Cell("C2").Value = creator;

            ws.Cell("B3").Value = "Loại Chứng từ:";
            ws.Cell("C3").Value = docType;

            ws.Cell("B4").Value = "Thời gian tạo:";
            ws.Cell("C4").Value = createdAt.ToString("dd/MM/yyyy HH:mm");

            // Lấy property của T
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Header cho bảng
            for (int i = 0; i < props.Length; i++)
            {
                ws.Cell(6, i + 2).Value = props[i].Name;
                ws.Cell(6, i + 2).Style.Font.Bold = true;
                ws.Cell(6, i + 2).Style.Fill.BackgroundColor = XLColor.LightGray;
                ws.Cell(6, i + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Ghi data
            int row = 7;
            foreach (var item in data)
            {
                for (int col = 0; col < props.Length; col++)
                {
                    var value = props[col].GetValue(item);
                    ws.Cell(row, col + 2).Value = value != null ? XLCellValue.FromObject(value) : XLCellValue.FromObject("");
                }
                row++;
            }

            // Autofit cột
            ws.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }
}