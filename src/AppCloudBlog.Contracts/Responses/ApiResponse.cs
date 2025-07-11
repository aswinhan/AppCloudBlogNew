namespace AppCloudBlog.Contracts.Responses;

// Generic API response for successful operations
public record ApiResponse<T>
{
    public bool IsSuccess { get; init; } = true; // Always true for successful responses
    public T? Data { get; init; } // The actual data payload
    // HttpStatusCode is handled by the Minimal API Results methods (e.g., Results.Ok, Results.Created)
    // ErrorMessages are handled by ProblemDetails for failures, not here.
}