namespace AppCloudBlog.API.Middleware;

// GlobalExceptionHandler implements IExceptionHandler to catch unhandled exceptions
// and return standardized ProblemDetails responses.
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Log the exception for internal monitoring
        _logger.LogError(
            exception,
            "An unhandled exception occurred: {ErrorMessage}",
            exception.Message);

        // Initialize ProblemDetails with a generic internal server error
        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "An unexpected error occurred.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1", // Standard for Internal Server Error
            Detail = "An unexpected error occurred. Please try again later."
        };

        // Handle specific custom exceptions and map them to appropriate HTTP status codes and details
        switch (exception)
        {
            case ValidationException validationException: // S_R1
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "One or more validation errors occurred.";
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"; // Standard for Bad Request
                // Add validation errors to extensions for client consumption
                problemDetails.Extensions["errors"] = validationException.Errors;
                break;
            case NotFoundException notFoundException:
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                problemDetails.Title = "Resource not found.";
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"; // Standard for Not Found
                problemDetails.Detail = notFoundException.Message;
                break;
            case UnauthorizedException unauthorizedException:
                problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                problemDetails.Title = "Authentication failed.";
                problemDetails.Type = "https://tools.ietf.org/html/rfc7235#section-3.1"; // Standard for Unauthorized
                problemDetails.Detail = unauthorizedException.Message;
                break;
            case ForbiddenException forbiddenException:
                problemDetails.Status = (int)HttpStatusCode.Forbidden;
                problemDetails.Title = "Access denied.";
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"; // Standard for Forbidden
                problemDetails.Detail = forbiddenException.Message;
                break;
            case ConflictException conflictException:
                problemDetails.Status = (int)HttpStatusCode.Conflict;
                problemDetails.Title = "Conflict occurred.";
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"; // Standard for Conflict
                problemDetails.Detail = conflictException.Message;
                break;
            default:
                // For any other unhandled exception, return 500 Internal Server Error
                // The default ProblemDetails already covers this.
                break;
        }

        // Set the response status code and content type
        httpContext.Response.StatusCode = problemDetails.Status.Value;
        httpContext.Response.ContentType = "application/problem+json";

        // Write the ProblemDetails response to the HTTP response body
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // Indicates that the exception has been handled
    }
}