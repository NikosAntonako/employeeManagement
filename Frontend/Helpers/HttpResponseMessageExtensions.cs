using Frontend.Models;
using System.Net.Http.Json;

namespace Frontend.Helpers;

public static class HttpResponseMessageExtensions
{
    public static async Task<string> GetErrorMessageAsync(this HttpResponseMessage response, string fallbackMessage)
    {
        ApiProblemDetails? problem = null;

        try
        {
            problem = await response.Content.ReadFromJsonAsync<ApiProblemDetails>();
        }
        catch
        {
        }

        var validationMessages = problem?.Errors?
            .SelectMany(entry => entry.Value)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .Distinct()
            .ToArray();

        if (validationMessages is { Length: > 0 })
            return string.Join(" ", validationMessages);

        if (!string.IsNullOrWhiteSpace(problem?.Detail))
            return problem.Detail;

        if (!string.IsNullOrWhiteSpace(problem?.Title))
            return problem.Title;

        return fallbackMessage;
    }
}
