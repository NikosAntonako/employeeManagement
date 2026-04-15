using EmployeeManagement.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.Json;

namespace Frontend.ViewModels
{
    /// <summary>
    /// Provides a base class for view models in Blazor applications, offering common properties and services for
    /// derived view models.
    /// </summary>
    /// <remarks>This class is intended to be inherited by view models that require access to Blazor framework
    /// services such as navigation, HTTP client creation, and JavaScript interop. It also provides properties for
    /// tracking initialization state and page metadata.</remarks>
    public class BaseViewModel
    {
        protected ComponentBase? _component;
        protected IHttpClientFactory HttpClientFactory { get; set; } = default!;
        protected NavigationManager Navigation { get; set; } = default!;
        protected IJSRuntime JS { get; set; } = default!;
        protected ILogger Logger { get; set; } = default!;
        public bool Initialized { get; set; }
        public string? PageTitle { get; set; }

        protected async Task<string> GetErrorMessageAsync(HttpResponseMessage response, string fallbackMessage)
        {
            try
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

                return string.IsNullOrWhiteSpace(apiResponse?.Message)
                    ? fallbackMessage
                    : $"{apiResponse.Message} {fallbackMessage}";
            }
            catch (Exception exception) when (exception is JsonException or NotSupportedException)
            {
                return fallbackMessage;
            }
        }
    }
}
