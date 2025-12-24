namespace Contract;

public class LogMessageBase
{
    public const string Created = "{EntityName} with ID {Id} has been created successfully.";
    public const string Updated = "{EntityName} with ID {Id} has been updated successfully.";
    public const string Deleted = "{EntityName} with ID {Id} has been deleted.";
    public const string Retrieved = "{EntityName} with ID {Id} has been retrieved.";
    public const string NotFound = "{EntityName} with ID {Id} was not found.";
    public const string OperationFailed = "Failed to perform operation on {EntityName} with ID {Id}";
    public const string UnauthorizedAccess = "Unauthorized access attempt to {EntityName} with ID {Id}.";
    public const string ValidationFailed = "Validation failed for {EntityName}. Reason: {ErrorMessage}";
    public const string Processing = "Processing {EntityName} with ID {Id}...";
    public const string OperationCompleted = "Operation on {EntityName} with ID {Id} completed successfully.";

    public static string Format(string message, params object[] values)
    => string.Format(message, values);
}
