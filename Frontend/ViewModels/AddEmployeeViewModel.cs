using Frontend.Models;
using Frontend.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Frontend.ViewModels;

/// <summary>
/// Represents the view model for adding a new employee, providing properties and methods to manage employee input,
/// submission, and navigation within the add employee workflow.
/// </summary>
/// <remarks>This view model is intended for use in UI components that facilitate the creation of new employees.
/// It manages form state, handles submission logic, and provides feedback messages for success or error scenarios. The
/// view model also exposes navigation functionality to return to the main page. Instances of this class are typically
/// used in Blazor or similar component-based frameworks.</remarks>
public class AddEmployeeViewModel : BaseViewModel
{
    public AddEmployeeViewModel(
        ComponentBase component,
        IHttpClientFactory httpClientFactory,
        NavigationManager navigation,
        IJSRuntime js)
    {
        _component = component;
        HttpClientFactory = httpClientFactory;
        Navigation = navigation;
        JS = js;
        _httpClient = HttpClientFactory.CreateClient("BackendApi");
        PageTitle = "Add New Employee";

        Initialized = true;
        //component.StateHasChanged();
    }

    public HttpClient _httpClient = default!;

    // 2. Fields and properties
    public readonly EmployeeInput employee = new();

    // Notification Fields
    public string? successMessage;
    public string? errorMessage;

    // Loading indicator true = on, false = off
    public bool isLoading = false;

    // 3. Event handlers and public methods
    public async Task HandleValidSubmit()
    {
        // Turn Loading on
        isLoading = true;
        // Clear previous error
        successMessage = errorMessage = null;

        try
        {
            var response = await _httpClient.PostAsJsonAsync("Employee/Create", employee);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<EmployeeViewModel>>();
                var createdEmployee = apiResponse?.Data;
                successMessage = $"New Employee '{createdEmployee?.Name ?? employee.Name}' " +
                    $"with id: {createdEmployee?.Id} was added successfully.";
            }
            else
            {
                errorMessage = await response.GetErrorMessageAsync($"Failed to add employee '{employee.Name}'. " +
                    $"Please check your input and try again.");
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

    public void GoBack()
    {
        Navigation.NavigateTo("/");
    }

    public void ResetForm()
    {
        employee.Name = "";
        employee.Position = "";
        employee.Department = "";
        employee.Salary = null;
    }
}
