using Microsoft.IdentityModel.Tokens;

namespace API;

internal sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            await HandleExceptionAsync(context, e);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var statusCode = GetStatusCode(exception);

        var response = new
        {
            title = GetTitle(exception),
            status = statusCode,
            detail = GetDetail(exception),
            errors = GetErrors(exception)
        };

        httpContext.Response.ContentType = "application/json";

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            FluentValidation.ValidationException => StatusCodes.Status400BadRequest,
            BadRequestException => StatusCodes.Status400BadRequest,
            ParameterInvalidException => StatusCodes.Status400BadRequest,
            UnauthenticatedException => StatusCodes.Status401Unauthorized,
            NoAuthenticationHeaderException => StatusCodes.Status401Unauthorized,
            TokenInvalidException => StatusCodes.Status401Unauthorized,
            TokenExpiredException => StatusCodes.Status401Unauthorized,
            ForbiddenException => StatusCodes.Status403Forbidden,
            NotFoundException => StatusCodes.Status404NotFound,
            FormatException => StatusCodes.Status422UnprocessableEntity,
            ServiceUnavailableException => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError
        };

    private static string GetDetail(Exception exception)
        => exception switch
        {
            FluentValidation.ValidationException => "One or more propety was validated.",
            _ => exception.Message
        };
    private static object GetTitle(Exception exception)
    => exception switch
    {
        DomainException applicationException => applicationException.Title,
        FluentValidation.ValidationException => "Validation failed",
        _ => "Server error"
    };

    private static List<object>? GetErrors(Exception exception)
    {
        return exception is ValidationException validationException
            ? validationException.Errors
                .Select(e => (object)new { e.PropertyName, e.ErrorMessage })
                .ToList()
            : null;
    }
}
