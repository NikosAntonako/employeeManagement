namespace Backend.Common;

/// <summary>
/// Represents the result of an operation, including success status, message, optional data, errors, and status code.
/// </summary>
/// <remarks>Use this class to encapsulate the outcome of an operation, providing both result data and contextual
/// information such as error messages and status codes. This pattern is useful for returning detailed results from
/// service or API methods.</remarks>
/// <typeparam name="T">The type of the data returned by the operation.</typeparam>
public sealed class Result<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public IReadOnlyCollection<string> Errors { get; init; } = [];
    public int StatusCode { get; init; }

    public static Result<T> CreateSuccess(T data, string message, int statusCode = StatusCodes.Status200OK)
    {
        return new Result<T>
        {
            Success = true,
            Message = message,
            Data = data,
            StatusCode = statusCode
        };
    }

    public static Result<T> Failure(string message, int statusCode, IEnumerable<string>? errors = null)
    {
        return new Result<T>
        {
            Success = false,
            Message = message,
            Errors = errors?.ToArray() ?? [],
            StatusCode = statusCode
        };
    }
}
