using EmployeeManagement.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Frontend.ViewModels;

/// <summary>
/// Represents the view model for adding a new employee, providing properties and methods to manage employee input,
/// department selection, and form submission in the add employee workflow.
/// </summary>
/// <remarks>This view model is intended for use in UI components that facilitate the creation of new employees.
/// It manages the state of the employee form, handles department suggestions, and coordinates navigation and
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
    public readonly EmployeeDto Employee = new()
    {
        Name = string.Empty,
        Position = string.Empty,
        DepartmentName = string.Empty
    };

    public IReadOnlyList<DepartmentDto> Departments { get; private set; } = [];
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsLoading { get; set; }

    public async Task InitializeAsync()
    {
        IsLoading = true;

        try
        {
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
            Employee.Name = Employee.Name.Trim();
            Employee.Position = Employee.Position.Trim();
            Employee.DepartmentName = Employee.DepartmentName.Trim();

            var response = await _httpClient.PostAsJsonAsync("Employee/Create", Employee);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<EmployeeResponseDto>>();
                var createdEmployee = apiResponse?.Data;
                SuccessMessage = $"Employee '{createdEmployee?.Name ?? Employee.Name}' was added successfully.";
                Navigation.NavigateTo($"/?success={Uri.EscapeDataString(SuccessMessage)}");
            }
            else
            {
                ErrorMessage = await GetErrorMessageAsync(
                    response,
                    $"We couldn't add employee '{Employee.Name}'. Please review the details and try again.");
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void ResetForm()
    {
        Employee.Name = string.Empty;
        Employee.Position = string.Empty;
        Employee.DepartmentName = string.Empty;
        Employee.Salary = null;
    }
    public void GoBack()
    {
        Navigation.NavigateTo("/");
    }
}
