using Frontend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Frontend.ViewModels;

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
    public EmployeeInput? Employee { get; set; }
    public EmployeeInput? OriginalEmployee { get; set; }
    public IReadOnlyList<DepartmentDto> Departments { get; private set; } = [];

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsLoading { get; set; }

    public async Task InitializeAsync()
    {
        var response = await _httpClient.GetAsync($"Employee/GetById/{Id}");

        if (!response.IsSuccessStatusCode)
        {
            ErrorMessage = "We couldn't load this employee.";
            return;
        }

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<EmployeeViewModel>>();
        var employeeData = apiResponse?.Data;

        if (employeeData == null)
        {
            ErrorMessage = "We couldn't load this employee.";
            return;
        }

        Employee = new EmployeeInput
        {
            Name = employeeData.Name,
            Position = employeeData.Position,
            DepartmentName = employeeData.Department,
            Salary = employeeData.Salary
        };

        OriginalEmployee = new EmployeeInput
        {
            Name = employeeData.Name,
            Position = employeeData.Position,
            DepartmentName = employeeData.Department,
            Salary = employeeData.Salary
        };

        await LoadDepartmentsAsync();
    }

    private async Task LoadDepartmentsAsync()
    {
        var response = await _httpClient.GetAsync("Department/GetAll");

        if (!response.IsSuccessStatusCode)
        {
            Departments = [];
            return;
        }

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<DepartmentDto>>>();
        Departments = apiResponse?.Data ?? [];
    }

    public async Task HandleValidSubmit()
    {
        IsLoading = true;
        SuccessMessage = ErrorMessage = null;

        try
        {
            if (Employee == null)
                return;

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
                    ErrorMessage = "We couldn't create the department. Please try again.";
                    return;
                }

                var createdDepartmentApiResponse =
                    await createDepartmentResponse.Content.ReadFromJsonAsync<ApiResponse<DepartmentDto>>();

                var createdDepartment = createdDepartmentApiResponse?.Data;

                if (createdDepartment == null)
                {
                    ErrorMessage = "We couldn't create the department. Please try again.";
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
                ErrorMessage = "We couldn't save the changes for this employee. Please try again.";
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
            Employee.DepartmentId = OriginalEmployee.DepartmentId;
            Employee.Salary = OriginalEmployee.Salary;
        }
    }

    public void GoBack()
    {
        Navigation.NavigateTo("/");
    }
}
