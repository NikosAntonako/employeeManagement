using Frontend.Models;
using System.Net.Http.Json;

namespace Frontend.Utilities;

public static class HttpResponseMessageExtensions
{
    public static async Task<string> GetErrorMessageAsync(this HttpResponseMessage response, string fallbackMessage)
    {
        try
        {
            var problemDetails = await response.Content.ReadFromJsonAsync<ApiProblemDetails>();

            var validationErrors = problemDetails?.Errors?
                .SelectMany(error => error.Value)
                .Where(message => !string.IsNullOrWhiteSpace(message))
                .ToArray();

            if (validationErrors is { Length: > 0 })
                return string.Join(" ", validationErrors);

            return problemDetails?.Detail
                ?? problemDetails?.Title
                ?? fallbackMessage;
        }
        catch
        {
            return fallbackMessage;
        }
    }
}
