using Backend.Common;

namespace Backend.Dtos;

/// <summary>
/// Represents a standardized response model that encapsulates the outcome of an operation, including success status,
/// message, data, and error details.
/// </summary>
/// <remarks>This model is commonly used to provide consistent API responses, enabling clients to easily interpret
/// operation results, access returned data, and handle errors. The model is immutable and can be constructed using the
/// provided static factory methods.</remarks>
/// <typeparam name="T">The type of the data returned by the operation.</typeparam>
public sealed class ResponseModel<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public IReadOnlyCollection<string> Errors { get; init; } = [];

    public static ResponseModel<T> FromResult(Result<T> result)
    {
        return new ResponseModel<T>
        {
            Success = result.Success,
            Message = result.Message,
            Data = result.Data,
            Errors = result.Errors
        };
    }

    public static ResponseModel<T> Failure(string message, IEnumerable<string>? errors = null)
    {
        return new ResponseModel<T>
        {
            Success = false,
            Message = message,
            Errors = errors?.ToArray() ?? []
        };
    }
}
