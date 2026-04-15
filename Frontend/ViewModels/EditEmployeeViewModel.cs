using EmployeeManagement.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net;
using System.Net.Http.Json;

namespace Frontend.ViewModels;

/// <summary>
/// Represents the view model for editing an employee, providing properties and methods to manage employee data, handle
/// form submission, and interact with related departments.
/// </summary>
/// <remarks>This view model is intended for use in UI components that allow editing of employee information. It
/// manages the retrieval and updating of employee data, department suggestions, and provides feedback
/// messages for success or error states. The view model also supports form reset and navigation actions. ALw
/// ll
/// asynchronous operations should be awaited to ensure proper UI updates.</remarks>
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
        Initialized = true;
    }

    private readonly HttpClient _httpClient = default!;

    public int Id { get; set; }
    public EmployeeDto? Employee { get; set; }
    public EmployeeDto? OriginalEmployee { get; set; }
    public IReadOnlyList<DepartmentDto> Departments { get; private set; } = [];
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public string? PageErrorMessage { get; private set; }
    public bool IsEmployeeNotFound { get; private set; }
    public bool IsLoading { get; set; }

    public async Task InitializeAsync()
    {
        IsLoading = true;
        IsEmployeeNotFound = false;
        ErrorMessage = null;
        PageErrorMessage = null;

        try
        {
            var response = await _httpClient.GetAsync($"Employee/GetById/{Id}");

            if (!response.IsSuccessStatusCode)
            {
                IsEmployeeNotFound = response.StatusCode == HttpStatusCode.NotFound;
                ErrorMessage = await GetErrorMessageAsync(response, "We couldn't load this employee.");
                PageErrorMessage = ErrorMessage;
                return;
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<EmployeeResponseDto>>();
            var employeeData = apiResponse?.Data;

            if (employeeData == null)
            {
                ErrorMessage = "We couldn't load this employee.";
                PageErrorMessage = ErrorMessage;
                return;
            }

            Employee = new EmployeeDto
            {
                Name = employeeData.Name,
                Position = employeeData.Position,
                DepartmentName = employeeData.Department,
                Salary = employeeData.Salary
            };

            OriginalEmployee = new EmployeeDto
            {
                Name = employeeData.Name,
                Position = employeeData.Position,
                DepartmentName = employeeData.Department,
                Salary = employeeData.Salary
            };

            await LoadDepartmentsAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadDepartmentsAsync()
    {
        var response = await _httpClient.GetAsync("Department/GetAll");

        if (!response.IsSuccessStatusCode)
        {
            Departments = [];
            ErrorMessage = await GetErrorMessageAsync(response, "We couldn't load departments.");
            return;
        }

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<DepartmentDto>>>();
        Departments = apiResponse?.Data?
            .OrderBy(department => department.Name)
            .ToArray() ?? [];
    }

    public async Task HandleValidSubmit()
    {
        IsLoading = true;
        SuccessMessage = ErrorMessage = null;

        try
        {
            if (Employee == null)
                return;

            Employee.Name = Employee.Name.Trim();
            Employee.Position = Employee.Position.Trim();
            Employee.DepartmentName = Employee.DepartmentName.Trim();

            var response = await _httpClient.PutAsJsonAsync($"Employee/Update/{Id}", Employee);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<EmployeeResponseDto>>();
                var updatedEmployee = apiResponse?.Data;
                SuccessMessage = $"Employee '{updatedEmployee?.Name ?? OriginalEmployee?.Name ?? Employee.Name}' was updated successfully.";
                Navigation.NavigateTo($"/?success={Uri.EscapeDataString(SuccessMessage)}");
            }
            else
            {
                ErrorMessage = await GetErrorMessageAsync(
                    response,
                    "We couldn't save the changes for this employee. Please try again.");
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void ResetForm()
    {
        if (Employee != null && OriginalEmployee != null)
        {
            Employee.Name = OriginalEmployee.Name;
            Employee.Position = OriginalEmployee.Position;
            Employee.DepartmentName = OriginalEmployee.DepartmentName;
            Employee.Salary = OriginalEmployee.Salary;
        }
    }

    public void GoBack()
    {
        Navigation.NavigateTo("/");
    }
}
