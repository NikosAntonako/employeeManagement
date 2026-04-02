using Frontend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Frontend.Pages;

public partial class AddEmployee : ComponentBase
{
    // 1.Injections
    [Inject] public IHttpClientFactory HttpClientFactory { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;
    [Inject] public IJSRuntime JS { get; set; } = default!;

    private HttpClient _httpClient = default!;

    // 2. Fields and properties
    private EmployeeInput employee = new();

    // Notification Fields
    private string? successMessage;
    private string? errorMessage;

    // Loading indicator true = on, false = off
    private bool isLoading = false;

    // 3. Event handlers and public methods
    private async Task HandleValidSubmit()
    {
        // Turn Loading on
        isLoading = true;
        // Clear previous error
        successMessage = errorMessage = null;

        try
        {
            var response = await _httpClient.PostAsJsonAsync("employee/Post", employee);

            if (response.IsSuccessStatusCode)
            {
                var createdEmployee = await response.Content.ReadFromJsonAsync<ApiResponse<EmployeeViewModel>>();
                successMessage = $"New Employee '{createdEmployee?.Data?.Name ?? employee.Name}' with id: {createdEmployee?.Data?.Id} was added successfully.";
            }
            else
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                errorMessage = errorResponse?.Message ?? $"Failed to add employee '{employee.Name}'. Please check your input and try again.";
            }
        }
        catch (Exception exception)
        {
            errorMessage = $"Error adding '{employee.Name}': {exception.Message}";
            Console.Error.WriteLine($"Error adding '{employee.Name}': {exception.Message}");
        }
        finally
        {
            // Turn Loading off
            isLoading = false;
        }
    }

    void GoBack()
    {
        Navigation.NavigateTo("/");
    }

    // 4. Life cycle methods
    protected override void OnInitialized()
    {
        _httpClient = HttpClientFactory.CreateClient("BackendApi");
    }
}
