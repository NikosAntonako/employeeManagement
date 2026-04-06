namespace Frontend.Models;

/// <summary>
/// Represents a standard response returned by an API, including status information, an optional message, and optional
/// data of a specified type.
/// </summary>
/// <remarks>Use this class to encapsulate the result of an API operation, providing both status and data in a
/// consistent format. This is commonly used to standardize responses across different API endpoints.</remarks>
/// <typeparam name="T">The type of the data included in the API response.</typeparam>
public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}
