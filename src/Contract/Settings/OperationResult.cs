namespace Contract;

public class OperationResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
	public object? Data { get; set; }

    public static OperationResult SuccessResult(string? message = null, object? data = null)
    {
        return new OperationResult
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static OperationResult FailureResult(string message)
    {
        return new OperationResult
        {
            Success = false,
            Message = message
        };
    }
	public static OperationResult PartialSuccess(string message)
	{
		return new OperationResult
		{
			Success = true,
			Message = message
		};
	}
}

