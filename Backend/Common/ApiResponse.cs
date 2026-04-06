using Microsoft.AspNetCore.WebUtilities;

namespace Backend.Common;

/// <summary>
/// Represents a standardized API response envelope. <see cref="Message"/>
/// is derived from either a custom detail message (if provided) or the HTTP reason phrase
/// via <see cref="ReasonPhrases"/>.
/// </summary>
/// <typeparam name="T">The type of the data payload included in the API response.</typeparam>
public sealed class ApiResponse<T>(
    int statusCode,
    T? data = default,
    string? detail = null)
{
    public int StatusCode { get; } = statusCode;
    public string Message { get; } = detail ?? ReasonPhrases.GetReasonPhrase(statusCode);
    public T? Data { get; } = data;
}
