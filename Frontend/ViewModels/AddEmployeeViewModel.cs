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

    private readonly HttpClient _httpClient = default!;

    // 2. Fields and properties
    public readonly EmployeeInput Employee = new();

    // Notification Fields
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    // Loading indicator true = on, false = off
    public bool IsLoading = false;

    // 3. Event handlers and public methods
    public async Task HandleValidSubmit()
    {
        // Turn Loading on
        IsLoading = true;
        // Clear previous error
        SuccessMessage = ErrorMessage = null;

        try
        {
            var response = await _httpClient.PostAsJsonAsync("Employee/Create", Employee);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<EmployeeViewModel>>();
                var createdEmployee = apiResponse?.Data;
                SuccessMessage = $"New Employee '{createdEmployee?.Name ?? Employee.Name}' " +
                    $"with id {createdEmployee?.Id} was added successfully.";
                Navigation.NavigateTo($"/?success={Uri.EscapeDataString(SuccessMessage)}");
            }
            else
            {
                ErrorMessage = await response.GetErrorMessageAsync($"Failed to add employee '{Employee.Name}'. " +
                    $"Please check your input and try again.");
            }
        }
        catch (HttpRequestException exception)
        {
            ErrorMessage = "Unable to connect to the server. Please try again later.";
            Console.Error.WriteLine($"API connection error adding '{Employee.Name}': {exception.Message}");
        }
        catch (Exception exception)
        {
            ErrorMessage = $"Something went wrong while adding '{Employee.Name}'. Please try again.";
            Console.Error.WriteLine($"Error adding '{Employee.Name}': {exception.Message}");
        }
        finally
        {
            // Turn Loading off
            IsLoading = false;
        }
    }

    public void GoBack()
    {
        Navigation.NavigateTo("/");
    }

    public void ResetForm()
    {
        Employee.Name = "";
        Employee.Position = "";
        Employee.Department = "";
        Employee.Salary = null;
    }
}
