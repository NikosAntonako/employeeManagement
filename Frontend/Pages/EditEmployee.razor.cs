using Frontend.Models;
using Frontend.Utilities;
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
            var response = await _httpClient.GetAsync($"Employee/GetById/{Id}");

            if (response.IsSuccessStatusCode)
            {
                var employeeData = await response.Content.ReadFromJsonAsync<EmployeeViewModel>();

                if (employeeData == null)
                {
                    errorMessage = $"Failed to load employee with id {Id}.";
                    return;
                }

                employee = new EmployeeInput
                {
                    Name = employeeData.Name,
                    Position = employeeData.Position,
                    Department = employeeData.Department,
                    Salary = employeeData.Salary
                };
            }
            else
            {
                errorMessage = await response.GetErrorMessageAsync($"Failed to load employee with id {Id}.");
            }
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
            var response = await _httpClient.PutAsJsonAsync($"Employee/Update/{Id}", employee);

            if (response.IsSuccessStatusCode)
            {
                var updatedEmployee = await response.Content.ReadFromJsonAsync<EmployeeViewModel>();
                successMessage = $"Employee '{updatedEmployee?.Name ?? employee?.Name}' with id {Id} was updated successfully.";
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
