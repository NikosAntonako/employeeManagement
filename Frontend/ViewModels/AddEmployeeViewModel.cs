using Frontend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Frontend.ViewModels;

/// <summary>
/// Represents the view model for adding a new employee, providing properties and methods to manage employee input,
/// department selection, and form submission in the add employee workflow.
/// </summary>
/// <remarks>This view model is intended for use in UI components that facilitate the creation of new employees.
/// It manages the state of the employee form, handles department creation if needed, and coordinates navigation and
/// messaging upon successful or failed operations. The class is not thread-safe and is designed for use within a single
/// UI context.</remarks>
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
    public bool IsLoading = false;

    public IReadOnlyList<DepartmentDto> Departments { get; private set; } = [];
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

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
        var response = await _httpClient.GetAsync("Department/GetAll");

        if (!response.IsSuccessStatusCode)
            return;

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<DepartmentDto>>>();
        Departments = apiResponse?.Data ?? [];
    }
}
