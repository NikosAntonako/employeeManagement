using Frontend.Models;
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
/// actions, and provides feedback messages for user actions. The class implements IDisposable to release resources
/// associated with search cancellation tokens. Thread safety is not guaranteed; use from a single UI thread.</remarks>
public class EmployeeListViewModel : BaseViewModel, IDisposable
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

        // Initialize
        Initialized = true;
    }

    private readonly HttpClient _httpClient = default!;

    // 2. Fields and properties
    public List<EmployeeViewModel>? Employees { get; set; }

    // Search Field
    private string _searchTerm = string.Empty;
    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (_searchTerm != value)
                _searchTerm = value;
        }
    }

    // Pagination Fields
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public int ActivePageIndex => Math.Max(CurrentPage - 1, 0);
    public int FirstItemNumber => Employees is { Count: > 0 } ? ((CurrentPage - 1) * PageSize) + 1 : 0;
    public int LastItemNumber => Employees is { Count: > 0 } ? FirstItemNumber + Employees.Count - 1 : 0;

    // Notification Fields
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    // Loading indicator true = on, false = off
    public bool IsLoading { get; set; } = false;

    // 3. Initialization method
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

    // 4. Event handlers and public methods
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
        // Clear previous notifications
        SuccessMessage = ErrorMessage = null;

        var employee = Employees?.FirstOrDefault(employee => employee.Id == id);
        string employeeName = employee?.Name ?? $"ID {id}";

        // Show confirmation dialog
        bool confirmed = await JS.InvokeAsync<bool>("confirm",
            $"Are you sure you want to delete employee '{employeeName}' with id {id}?");

        if (!confirmed)
            return;

        // Turn Loading on (after confirmation)
        IsLoading = true;

        try
        {
            var response = await _httpClient.DeleteAsync($"Employee/Delete/{id}");

            if (response.IsSuccessStatusCode)
            {
                if (employee != null && Employees?.Count == 1 && CurrentPage > 1)
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

    public async Task DeleteEmployeeConfirmedAsync(int id)
    {
        SuccessMessage = ErrorMessage = null;

        var employee = Employees?.FirstOrDefault(employee => employee.Id == id);
        string employeeName = employee?.Name ?? $"ID {id}";

        IsLoading = true;

        try
        {
            var response = await _httpClient.DeleteAsync($"Employee/Delete/{id}");

            if (response.IsSuccessStatusCode)
            {
                if (employee != null && Employees?.Count == 1 && CurrentPage > 1)
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

    // 5. Private helper methods
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
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult>>();
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

    private CancellationTokenSource? _searchCancellationTokenSource;

    public async Task OnSearchInput(ChangeEventArgs e)
    {
        SearchTerm = e.Value?.ToString() ?? string.Empty;
        CurrentPage = 1; // Reset to first page on search

        _searchCancellationTokenSource?.Cancel();
        _searchCancellationTokenSource?.Dispose();
        _searchCancellationTokenSource = new();

        try
        {
            await Task.Delay(250, _searchCancellationTokenSource.Token);
            await LoadEmployees();
        }
        catch (OperationCanceledException)
        {
        }
    }

    public async Task SetPageSizeAsync(int newSize)
    {
        if (PageSize != newSize)
        {
            PageSize = newSize;
            CurrentPage = 1;
            await LoadEmployees();
        }
    }

    public void Dispose()
    {
        _searchCancellationTokenSource?.Cancel();
        _searchCancellationTokenSource?.Dispose();
    }
}
