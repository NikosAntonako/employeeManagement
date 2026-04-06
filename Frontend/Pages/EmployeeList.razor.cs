using Frontend.Models;
using Frontend.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Frontend.Pages;

public partial class EmployeeList : ComponentBase, IDisposable
{
    // 1.Injections
    [Inject] public IHttpClientFactory HttpClientFactory { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;
    [Inject] public IJSRuntime JS { get; set; } = default!;

    private HttpClient _httpClient = default!;

    // 2. Fields and properties
    private List<EmployeeViewModel>? employees;

    // Search Field (backing field + property)
    private string _searchTerm = string.Empty;
    private string SearchTerm
    {
        get => _searchTerm;
        set
        {
            // Avoid re-renders until value changes
            if (_searchTerm != value)
                _searchTerm = value;
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

    // 3. Lifecycle methods
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

    // 4. Event handlers and public methods
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
        // Clear previous notifications
        successMessage = errorMessage = null;

        var employee = employees?.FirstOrDefault(employee => employee.Id == id);
        string employeeName = employee?.Name ?? $"ID {id}";

        // Show confirmation dialog
        bool confirmed = await JS.InvokeAsync<bool>("confirm", $"Are you sure you want to delete employee '{employeeName}' with id {id}?");

        if (!confirmed)
            return;

        // Turn Loading on (after confirmation)
        isLoading = true;

        try
        {
            var response = await _httpClient.DeleteAsync($"Employee/Delete/{id}");

            if (response.IsSuccessStatusCode)
            {
                if (employee != null && employees?.Count == 1 && currentPage > 1)
                    currentPage--;

                await LoadEmployees();
                successMessage = $"Employee '{employeeName}' with id {id} was deleted successfully.";
            }
            else
            {
                errorMessage = await response.GetErrorMessageAsync($"Failed to delete employee '{employeeName}' with id {id}.");
            }
        }
        catch (Exception exception)
        {
            errorMessage = $"Error deleting '{employeeName}' with id {id}: {exception.Message}";
            Console.Error.WriteLine($"Error deleting '{employeeName}' with id {id}: {exception.Message}");
        }
        finally
        {
            isLoading = false;
        }
    }

    // 5. Private helper methods
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

            var url = QueryHelpers.AddQueryString("Employee/GetAll", query);

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult>>();
                var result = apiResponse?.Data;
                employees = result?.Items.ToList() ?? [];
                totalPages = result?.TotalPages ?? 1;
                errorMessage = null;
            }
            else
            {
                employees = [];
                totalPages = 1;
                errorMessage = await response.GetErrorMessageAsync("Failed to load employees.");
            }
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

    private CancellationTokenSource? _searchCancellationTokenSource;

    private async Task OnSearchInput(ChangeEventArgs eventData)
    {
        SearchTerm = eventData.Value?.ToString() ?? string.Empty;
        currentPage = 1;

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
        catch (Exception exception)
        {
            errorMessage = $"Search failed: {exception.Message}";
            employees = [];
        }
    }

    public void Dispose()
    {
        _searchCancellationTokenSource?.Cancel();
        _searchCancellationTokenSource?.Dispose();
        GC.SuppressFinalize(this);
    }
}
