namespace Frontend.Models;

/// <summary>
/// Represents a standardized format for returning problem details in HTTP API error responses.
/// </summary>
/// <remarks>This class is typically used to convey machine-readable error information in a consistent structure,
/// following the RFC 7807 specification for problem details. It is commonly returned by web APIs to provide clients
/// with detailed error context, including a short, human-readable summary, a detailed description, and a collection of
/// validation or processing errors.</remarks>
public sealed class ApiProblemDetails
{
    public string? Title { get; set; }
    public string? Detail { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
}
