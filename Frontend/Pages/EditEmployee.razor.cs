using Frontend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Frontend.Pages;

public partial class EditEmployee : ComponentBase
{
    // 1.Injections
    [Inject] public IHttpClientFactory HttpClientFactory { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;
    [Inject] public IJSRuntime JS { get; set; } = default!;

    private HttpClient _httpClient = default!;

    // 2. Parameters
    [Parameter]
    public int Id { get; set; }

    // 3. Fields and properties
    private EmployeeInput? employee;

    // Notification Fields
    private CancellationTokenSource? messageCts;
    private string? successMessage;
    private string? errorMessage;

    // Loading indicator true = on, false = off
    private bool isLoading = false;

    // 4. Lifecycle methods
    protected override async Task OnInitializedAsync()
    {
        _httpClient = HttpClientFactory.CreateClient("BackendApi");

        try
        {
            employee = await _httpClient.GetFromJsonAsync<EmployeeInput>($"employees/{Id}");

        }
        catch (Exception exception)
        {
            errorMessage = "Failed to load employee: " + exception.Message;
        }
    }

    // 5. Event handlers and public methods
    private async Task HandleValidSubmit()
    {
        // Turn Loading on
        isLoading = true;
        // Clear previous error
        successMessage = errorMessage = null;

        try
        {
            var response = await _httpClient.PutAsJsonAsync($"employees/{Id}", employee);

            if (response.IsSuccessStatusCode)
            {
                successMessage = $"Employee  with id {Id} was updated successfully.";
            }
            else
            {
                errorMessage = $"Failed to update employee with id {Id}.";
            }
        }
        catch (Exception exception)
        {
            errorMessage = $"Error updating: {exception.Message}";
            Console.Error.WriteLine($"Error updating: {exception.Message}");
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
}
