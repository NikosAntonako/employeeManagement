using Frontend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Frontend.ViewModels;

/// <summary>
/// Represents the view model for adding a new employee, providing properties and methods to manage employee input,
/// submission, and navigation within the add employee workflow.
/// </summary>
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

    public readonly EmployeeInput Employee = new();

    public IReadOnlyList<DepartmentDto> Departments { get; private set; } = [];

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public bool IsLoading = false;

    public async Task InitializeAsync()
    {
        await LoadDepartmentsAsync();
    }

    public async Task HandleValidSubmit()
    {
        IsLoading = true;
        SuccessMessage = ErrorMessage = null;

        try
        {
            Employee.DepartmentName = Employee.DepartmentName.Trim();

            var existingDepartment = Departments.FirstOrDefault(department =>
                string.Equals(department.Name, Employee.DepartmentName, StringComparison.OrdinalIgnoreCase));

            if (existingDepartment != null)
            {
                Employee.DepartmentId = existingDepartment.Id;
            }
            else
            {
                var createDepartmentResponse = await _httpClient.PostAsJsonAsync(
                    "Department/Create",
                    new CreateDepartmentInput
                    {
                        Name = Employee.DepartmentName.Trim()
                    });

                if (!createDepartmentResponse.IsSuccessStatusCode)
                {
                    ErrorMessage = $"We couldn't create department '{Employee.DepartmentName}'.";
                    return;
                }

                var createdDepartmentApiResponse =
                    await createDepartmentResponse.Content.ReadFromJsonAsync<ApiResponse<DepartmentDto>>();

                var createdDepartment = createdDepartmentApiResponse?.Data;

                if (createdDepartment == null)
                {
                    ErrorMessage = $"We couldn't create department '{Employee.DepartmentName}'.";
                    return;
                }

                Employee.DepartmentId = createdDepartment.Id;
                Employee.DepartmentName = createdDepartment.Name;
                Departments = Departments
                    .Append(createdDepartment)
                    .DistinctBy(department => department.Name, StringComparer.OrdinalIgnoreCase)
                    .OrderBy(department => department.Name)
                    .ToArray();
            }

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
                ErrorMessage = $"We couldn't add employee '{Employee.Name}'. Please review the details and try again.";
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
        Employee.DepartmentName = string.Empty;
        Employee.DepartmentId = 0;
        Employee.Salary = null;
    }

    private async Task LoadDepartmentsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("Department/GetAll");

            if (!response.IsSuccessStatusCode)
                return;

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<DepartmentDto>>>();
            Departments = apiResponse?.Data ?? [];
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
}
