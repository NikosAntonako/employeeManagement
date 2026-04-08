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
        ILogger<EditEmployeeViewModel> logger,
        int employeeId)
    {
        _component = component;
        HttpClientFactory = httpClientFactory;
        Navigation = navigation;
        JS = js;
        Logger = logger;
        _httpClient = HttpClientFactory.CreateClient("BackendApi");
        Id = employeeId;
        PageTitle = "Edit Employee";

        // initialize
        Initialized = true;
    }

    private readonly HttpClient _httpClient = default!;

    // 2. Parameters and fields
    public int Id { get; set; }
    public EmployeeInput? Employee { get; set; }
    public EmployeeInput? OriginalEmployee { get; set; }
    public IReadOnlyList<string> Departments { get; private set; } = [];

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
                    ErrorMessage = "We couldn't load this employee.";
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
                OriginalEmployee = new EmployeeInput
                {
                    Name = employeeData.Name,
                    Position = employeeData.Position,
                    Department = employeeData.Department,
                    Salary = employeeData.Salary
                };

                await LoadDepartmentsAsync();
            }
            else
            {
                ErrorMessage = await response.GetErrorMessageAsync("We couldn't load this employee.");
            }
        }
        catch (HttpRequestException exception)
        {
            ErrorMessage = "We couldn't load this employee right now. Please try again later.";
            Logger.LogWarning(exception, "HTTP request failed while loading employee {EmployeeId}", Id);
        }
        catch (Exception exception)
        {
            if (IsConnectivityError(exception))
            {
                ErrorMessage = "We couldn't load this employee right now. Please try again later.";
                Logger.LogWarning(exception, "Connectivity issue while loading employee {EmployeeId}", Id);
            }
            else
            {
                ErrorMessage = "Something went wrong while loading this employee. Please try again.";
                Logger.LogError(exception, "Unexpected error while loading employee {EmployeeId}", Id);
            }
        }
    }

    private async Task LoadDepartmentsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("Employee/GetAll?pageNumber=1&pageSize=1000");

            if (!response.IsSuccessStatusCode)
            {
                Departments = Employee is not null && !string.IsNullOrWhiteSpace(Employee.Department)
                    ? [Employee.Department]
                    : [];
                return;
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult>>();
            var employees = apiResponse?.Data?.Items ?? [];

            Departments = employees
                .Select(employee => employee.Department)
                .Append(Employee?.Department ?? string.Empty)
                .Where(department => !string.IsNullOrWhiteSpace(department))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(department => department)
                .ToArray();
        }
        catch (HttpRequestException exception)
        {
            Logger.LogWarning(exception, "HTTP request failed while loading departments for Edit Employee {EmployeeId}", Id);
            Departments = Employee is not null && !string.IsNullOrWhiteSpace(Employee.Department)
                ? [Employee.Department]
                : [];
        }
        catch (Exception exception)
        {
            if (IsConnectivityError(exception))
                Logger.LogWarning(exception, "Connectivity issue while loading departments for Edit Employee {EmployeeId}", Id);
            else
                Logger.LogError(exception, "Unexpected error while loading departments for Edit Employee {EmployeeId}", Id);

            Departments = Employee is not null && !string.IsNullOrWhiteSpace(Employee.Department)
                ? [Employee.Department]
                : [];
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
                SuccessMessage = $"Employee '{updatedEmployee?.Name ?? OriginalEmployee?.Name}' was updated successfully.";
                Navigation.NavigateTo($"/?success={Uri.EscapeDataString(SuccessMessage)}");
            }
            else
            {
                ErrorMessage = await response.GetErrorMessageAsync(
                    "We couldn't save the changes for this employee. Please try again.");
            }
        }
        catch (HttpRequestException exception)
        {
            ErrorMessage = "We couldn't save the changes right now. Please try again later.";
            Logger.LogWarning(exception, "HTTP request failed while updating employee {EmployeeId}", Id);
        }
        catch (Exception exception)
        {
            if (IsConnectivityError(exception))
            {
                ErrorMessage = "We couldn't save the changes right now. Please try again later.";
                Logger.LogWarning(exception, "Connectivity issue while updating employee {EmployeeId}", Id);
            }
            else
            {
                ErrorMessage = "Something went wrong while saving the changes. Please try again.";
                Logger.LogError(exception, "Unexpected error while updating employee {EmployeeId}", Id);
            }
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
            Employee.Name = OriginalEmployee.Name;
            Employee.Position = OriginalEmployee.Position;
            Employee.Department = OriginalEmployee.Department;
            Employee.Salary = OriginalEmployee.Salary;
        }
    }

    public void GoBack()
    {
        Navigation.NavigateTo("/");
    }
}
