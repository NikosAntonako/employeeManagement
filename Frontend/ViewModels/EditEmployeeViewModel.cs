using Frontend.Models;
using Frontend.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Frontend.ViewModels;

/// <summary>
/// Represents the view model for editing an employee in the application, providing properties and methods to load,
/// update, and manage employee data and related UI state.
/// </summary>
/// <remarks>This view model is intended for use in Blazor components that handle editing employee information. It
/// manages the retrieval and update of employee data via HTTP requests, and exposes properties for tracking loading
/// state and user notifications. The view model also provides navigation support to return to the main page after
/// editing. All operations are asynchronous where network communication is involved.</remarks>
public class EditEmployeeViewModel : BaseViewModel
{
    public EditEmployeeViewModel(
        ComponentBase component,
        IHttpClientFactory httpClientFactory,
        NavigationManager navigation,
        IJSRuntime js,
        int employeeId)
    {
        _component = component;
        HttpClientFactory = httpClientFactory;
        Navigation = navigation;
        JS = js;
        _httpClient = HttpClientFactory.CreateClient("BackendApi");
        Id = employeeId;
        PageTitle = "Edit Employee";

        // initialize
        Initialized = true;
    }

    public HttpClient _httpClient = default!;

    // 2. Parameters and fields
    public int Id { get; set; }
    public EmployeeInput? Employee { get; set; }
    public EmployeeInput? OriginalEmployee { get; set; }

    // Notification Fields
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    // Loading indicator true = on, false = off
    public bool IsLoading { get; set; } = false;

    // 3. Initialization method
    public async Task InitializeAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"Employee/GetById/{Id}");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<EmployeeViewModel>>();
                var employeeData = apiResponse?.Data;

                if (employeeData == null)
                {
                    ErrorMessage = $"Failed to load employee with id {Id}.";
                    return;
                }

                // Map employeeData to EmployeeInput
                Employee = new EmployeeInput
                {
                    Name = employeeData.Name,
                    Position = employeeData.Position,
                    Department = employeeData.Department,
                    Salary = employeeData.Salary
                };
                OriginalEmployee = Employee.Clone();
            }
            else
            {
                ErrorMessage = await response.GetErrorMessageAsync($"Failed to load employee with id {Id}.");
            }
        }
        catch (Exception exception)
        {
            ErrorMessage = "Failed to load employee: " + exception.Message;
        }
    }

    // 4. Event handlers and public methods
    public async Task HandleValidSubmit()
    {
        // Turn Loading on
        IsLoading = true;
        // Clear previous notifications
        SuccessMessage = ErrorMessage = null;

        try
        {
            var response = await _httpClient.PutAsJsonAsync($"Employee/Update/{Id}", Employee);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<EmployeeViewModel>>();
                var updatedEmployee = apiResponse?.Data;
                SuccessMessage = $"Employee '{updatedEmployee?.Name ?? OriginalEmployee?.Name}' with id {Id} was updated successfully.";
            }
            else
            {
                ErrorMessage = await response.GetErrorMessageAsync($"Failed to update employee with id {Id}.");
            }
        }
        catch (Exception exception)
        {
            ErrorMessage = $"Error updating: {exception.Message}";
            Console.Error.WriteLine($"Error updating: {exception.Message}");
        }
        finally
        {
            // Turn Loading off
            IsLoading = false;
        }
    }

    public void ResetForm()
    {
        if (Employee != null && OriginalEmployee != null)
        {
            Employee = OriginalEmployee.Clone();
        }
    }

    public void GoBack()
    {
        Navigation.NavigateTo("/");
    }
}
