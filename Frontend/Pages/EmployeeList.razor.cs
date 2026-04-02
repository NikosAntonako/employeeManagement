using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Frontend.Pages;

public partial class EmployeeList : ComponentBase
{
    // 1.Injections
    [Inject] public IHttpClientFactory HttpClientFactory { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;
    [Inject] public IJSRuntime JS { get; set; } = default!;

    private HttpClient _httpClient = default!;

    // 2. Fields and properties
    private List<Employee>? employees;

    // Search Field (backing field + property)
    private string _searchTerm = string.Empty;
    private string SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (_searchTerm != value)
            {
                _searchTerm = value;
                currentPage = 1; // Reset to first page on new search
                _ = LoadEmployees();
            }
        }
    }

    // Pagination Fields
    private int currentPage = 1;
    private readonly int pageSize = 10;
    private int totalPages;

    // Notification Fields
    private string? successMessage;
    private string? errorMessage;

    // Loading indicator true = on, false = off
    private bool isLoading = false;

    // Retrieve API pagination
    public class PagedResult<T>
    {
        public required List<T> Items { get; set; }
        public int TotalPages { get; set; }
    }

    // 3. Parameters (if any)
    // [Parameter] public int SomeParameter { get; set; }

    // 4. Lifecycle methods
    protected override async Task OnInitializedAsync()
    {
        _httpClient = HttpClientFactory.CreateClient("BackendApi");
        var uri = Navigation.ToAbsoluteUri(Navigation.Uri);

        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("success", out var message))
            successMessage = message;

        try
        {
            await LoadEmployees();
        }
        catch (HttpRequestException exception) when (exception.Message.Contains("CORS", StringComparison.OrdinalIgnoreCase))
        {
            // CORS error handler
            employees = [];
            Console.Error.WriteLine("CORS error: " + exception.Message);
        }
        catch (Exception exception)
        {
            employees = [];
            Console.Error.WriteLine("General error: " + exception.Message);
        }
    }

    // 5. Event handlers and public methods
    void AddEmployee()
    {
        Navigation.NavigateTo("/add");
    }

    void EditEmployee(int id)
    {
        Navigation.NavigateTo($"/edit/{id}");
    }

    async Task DeleteEmployee(int id)
    {
        // Turn Loading on
        isLoading = true;
        // Clear previous notification
        successMessage = errorMessage = null;

        var employee = employees?.FirstOrDefault(employee => employee.Id == id);
        string employeeName = employee?.Name ?? $"ID {id}";

        // Show confirmation dialog
        bool confirmed = await JS.InvokeAsync<bool>("confirm", $"Are you sure you want to delete employee '{employeeName}' with id {id}?");

        if (!confirmed)
        {
            isLoading = false;
            return;
        }

        try
        {
            var response = await _httpClient.DeleteAsync($"employee/Delete{id}");

            if (response.IsSuccessStatusCode)
            {
                // Re-fetching the list from the backend
                await LoadEmployees();
                successMessage = $"Employee '{employeeName}' with id {id} was deleted successfully.";
            }
            else
            {
                errorMessage = $"Failed to delete employee '{employeeName}' with id {id}. Maybe it was already deleted.";
            }
        }
        catch (Exception exception)
        {
            errorMessage = $"Error deleting '{employeeName}' with id {id}: {exception.Message}";
            Console.Error.WriteLine($"Error deleting '{employeeName}' with id {id}: {exception.Message}");
        }
        finally
        {
            // Turn Loading off
            isLoading = false;
        }
    }

    // 6. Private helper methods
    // Load Results using Pagination settings
    private async Task LoadEmployees()
    {
        isLoading = true;

        try
        {
            var query = new Dictionary<string, string?>
            {
                ["pageNumber"] = currentPage.ToString(),
                ["pageSize"] = pageSize.ToString()
            };

            if (!string.IsNullOrWhiteSpace(SearchTerm))
                query["searchTerm"] = SearchTerm;

            var url = QueryHelpers.AddQueryString("employee/GetAll", query);

            var response = await _httpClient.GetFromJsonAsync<PagedResult<Employee>>(url);
            employees = response?.Items ?? [];
            totalPages = response?.TotalPages ?? 1;
        }
        finally
        {
            isLoading = false;
        }
    }

    // Change Page
    private async Task GoToPage(int page)
    {
        if (page < 1 || page > totalPages)
            return;

        currentPage = page;
        await LoadEmployees();
    }

    private CancellationTokenSource? _searchCts;

    private async Task OnSearchInput(ChangeEventArgs e)
    {
        SearchTerm = e.Value?.ToString() ?? string.Empty;
        currentPage = 1;

        _searchCts?.Cancel();
        _searchCts = new();

        try
        {
            await Task.Delay(250, _searchCts.Token);
            await LoadEmployees();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            errorMessage = $"Search failed: {exception.Message}";
            employees = [];
        }
    }

    // 7. Nested classes
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal Salary { get; set; }
    }
}
