namespace Application;

public interface IExcelExporter<T>
{
    byte[] ExportToExcel(IEnumerable<T> data, string sheetName = "Data");
    byte[] ExportToExcel(Dictionary<string, IEnumerable<T>> dataBySheet);
    byte[] ExportInvoice(List<T> products, string store, string creator, string docType, DateTime createdAt);
}
