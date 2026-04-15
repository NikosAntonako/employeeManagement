using Microsoft.AspNetCore.WebUtilities;

namespace EmployeeManagement.Shared;

/// <summary>
/// Represents a standardized API response envelope.
/// </summary>
public sealed record ApiResponse<T>(
    int StatusCode,
    T? Data = default,
    string? Detail = null)
{
    public bool IsSuccess => StatusCode is >= 200 and < 300;
    public string Message => Detail ?? ReasonPhrases.GetReasonPhrase(StatusCode);
}
