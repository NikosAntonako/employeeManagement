using Microsoft.AspNetCore.WebUtilities;

namespace Backend.Common;

/// <summary>
/// Represents a standardized API response envelope.
/// </summary>
public sealed class ApiResponse<T>(
    int statusCode,
    T? data = default,
    string? detail = null)
{
    public int StatusCode { get; } = statusCode;
    public string Message { get; } = detail ?? ReasonPhrases.GetReasonPhrase(statusCode);
    public T? Data { get; } = data;
}
