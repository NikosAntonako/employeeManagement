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
        IJSRuntime js,
        ILogger<AddEmployeeViewModel> logger)
    {
        _component = component;
        HttpClientFactory = httpClientFactory;
        Navigation = navigation;
        JS = js;
        Logger = logger;
        _httpClient = HttpClientFactory.CreateClient("BackendApi");
        PageTitle = "Add New Employee";

        Initialized = true;
    }

    private readonly HttpClient _httpClient = default!;

    // 2. Fields and properties
    public readonly EmployeeInput Employee = new();

    public IReadOnlyList<string> Departments { get; private set; } = [];

    // Notification Fields
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    // Loading indicator true = on, false = off
    public bool IsLoading = false;

    // 3. Event handlers and public methods
    public async Task InitializeAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("Employee/GetAll?pageNumber=1&pageSize=1000");

            if (!response.IsSuccessStatusCode)
                return;

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult>>();
            var employees = apiResponse?.Data?.Items ?? [];

            Departments = employees
                .Select(employee => employee.Department)
                .Where(department => !string.IsNullOrWhiteSpace(department))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(department => department)
                .ToArray();
        }
        catch (HttpRequestException exception)
        {
            Logger.LogWarning(exception, "HTTP request failed while loading departments for Add Employee");
        }
        catch (Exception exception)
        {
            if (IsConnectivityError(exception))
                Logger.LogWarning(exception, "Connectivity issue while loading departments for Add Employee");
            else
                Logger.LogError(exception, "Unexpected error while loading departments for Add Employee");
        }
    }

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
                SuccessMessage = $"Employee '{createdEmployee?.Name ?? Employee.Name}' was added successfully.";
                Navigation.NavigateTo($"/?success={Uri.EscapeDataString(SuccessMessage)}");
            }
            else
            {
                ErrorMessage = await response.GetErrorMessageAsync(
                    $"We couldn't add employee '{Employee.Name}'. Please review the details and try again.");
            }
        }
        catch (HttpRequestException exception)
        {
            ErrorMessage = $"We couldn't add employee '{Employee.Name}' right now. Please try again later.";
            Logger.LogWarning(exception, "HTTP request failed while adding employee {EmployeeName}", Employee.Name);
        }
        catch (Exception exception)
        {
            if (IsConnectivityError(exception))
            {
                ErrorMessage = $"We couldn't add employee '{Employee.Name}' right now. Please try again later.";
                Logger.LogWarning(exception, "Connectivity issue while adding employee {EmployeeName}", Employee.Name);
            }
            else
            {
                ErrorMessage = $"Something went wrong while adding employee '{Employee.Name}'. Please try again.";
                Logger.LogError(exception, "Unexpected error while adding employee {EmployeeName}", Employee.Name);
            }
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
        Employee.Name = string.Empty;
        Employee.Position = string.Empty;
        Employee.Department = string.Empty;
        Employee.Salary = null;
    }
}
