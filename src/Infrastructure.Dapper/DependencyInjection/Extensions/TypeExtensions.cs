using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Dapper;

public static class TypeExtensions
{
    public static string GetDbTableName(this Type type)
    {
        var tableAttribute = type.GetCustomAttributes(typeof(TableAttribute), false)
                                 .FirstOrDefault() as TableAttribute;
        return tableAttribute?.Name ?? type.Name;
    }

    public static string GetDbTableName(this Type type, string[] columns)
    {
        return string.Join(", ", columns);
    }
    public static string GetDbTableColumnNames(this Type type)
    {
        var columnAttribute = type.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute), false)
                                 .FirstOrDefault() as System.ComponentModel.DataAnnotations.Schema.ColumnAttribute;
        return columnAttribute?.Name ?? type.Name;
    }

    public static string GetColumnValuesForInsert(this Type type)
    {
        var properties = type.GetProperties();
        var columnValues = properties.Select(p => p.GetValue(type)?.ToString() ?? string.Empty);
        return string.Join(", ", columnValues);
    }
}
