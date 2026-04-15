using EmployeeManagement.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Frontend.ViewModels;

/// <summary>
/// Represents the view model for displaying and managing a paginated list of employees, including search, add, edit,
/// and delete operations.
/// </summary>
/// <remarks>This view model is intended for use in UI components that display employee data with support for
/// pagination, searching, and user notifications. It manages the loading state, handles navigation for add and edit
/// actions, and provides feedback messages for user actions. Thread safety is not guaranteed; use from a single UI
/// thread.</remarks>
public class EmployeeListViewModel : BaseViewModel
{
    public EmployeeListViewModel(
        ComponentBase component,
        IHttpClientFactory httpClientFactory,
        NavigationManager navigation,
        IJSRuntime js,
        ILogger<EmployeeListViewModel> logger)
    {
        _component = component;
        HttpClientFactory = httpClientFactory;
        Navigation = navigation;
        JS = js;
        Logger = logger;
        _httpClient = HttpClientFactory.CreateClient("BackendApi");
        PageTitle = "Employee List";
        Initialized = true;
    }

    private readonly HttpClient _httpClient = default!;
    private string _searchTerm = string.Empty;

    public List<EmployeeResponseDto> Employees { get; set; } = [];

    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (_searchTerm != value)
                _searchTerm = value;
        }
    }

    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }

    public int ActivePageIndex
    {
        get => Math.Max(CurrentPage - 1, 0);
        set => CurrentPage = value + 1;
    }

    public int FirstItemNumber => Employees.Count == 0 ? 0 : ((CurrentPage - 1) * PageSize) + 1;
    public int LastItemNumber => Employees.Count == 0 ? 0 : FirstItemNumber + Employees.Count - 1;
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsLoading { get; set; }

    public async Task InitializeAsync(string? currentUri)
    {
        if (!string.IsNullOrEmpty(currentUri))
        {
            var uri = new Uri(currentUri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("success", out var message))
                SuccessMessage = message;
        }

        await LoadEmployees();
    }

    public void AddEmployee()
    {
        Navigation.NavigateTo("/add");
    }

    public void EditEmployee(int id)
    {
        Navigation.NavigateTo($"/edit/{id}");
    }

    public async Task DeleteEmployee(int id)
    {
        SuccessMessage = ErrorMessage = null;

        var employee = Employees.FirstOrDefault(employee => employee.Id == id);
        string employeeName = employee?.Name ?? $"ID {id}";

        IsLoading = true;

        try
        {
            var response = await _httpClient.DeleteAsync($"Employee/Delete/{id}");

            if (response.IsSuccessStatusCode)
            {
                if (employee is not null && Employees.Count == 1 && CurrentPage > 1)
                    CurrentPage--;

                await LoadEmployees();
                SuccessMessage = $"Employee '{employeeName}' was deleted successfully.";
            }
            else
            {
                ErrorMessage = $"We couldn't delete employee '{employeeName}'. Please try again.";
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task LoadEmployees()
    {
        IsLoading = true;

        try
        {
            var query = new Dictionary<string, string?>
            {
                ["pageNumber"] = CurrentPage.ToString(),
                ["pageSize"] = PageSize.ToString()
            };

            if (!string.IsNullOrWhiteSpace(SearchTerm))
                query["searchTerm"] = SearchTerm;

            var url = QueryHelpers.AddQueryString("Employee/GetAll", query);

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto>>();
                var result = apiResponse?.Data;
                Employees = result?.Items.ToList() ?? [];
                TotalCount = result?.TotalCount ?? 0;
                TotalPages = result?.TotalPages ?? 0;
                CurrentPage = result?.CurrentPage ?? 1;
                PageSize = result?.PageSize ?? PageSize;
                ErrorMessage = null;
            }
            else
            {
                Employees = [];
                TotalCount = 0;
                TotalPages = 0;
                ErrorMessage = "We couldn't load the employee list.";
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task GoToPage(int page)
    {
        if (page < 1 || (TotalPages > 0 && page > TotalPages))
            return;

        CurrentPage = page;
        await LoadEmployees();
    }

    public async Task OnSearchInput(ChangeEventArgs e)
    {
        SearchTerm = e.Value?.ToString() ?? string.Empty;
        CurrentPage = 1;
        await LoadEmployees();
    }

    public async Task SetPageSize(int newSize)
    {
        if (PageSize != newSize)
        {
            PageSize = newSize;
            CurrentPage = 1;
            await LoadEmployees();
        }
    }
}
