using Frontend.Models;
using Frontend.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Frontend.ViewModels;

public class AddEmployeeViewModel : BaseViewModel
{
    public AddEmployeeViewModel(ComponentBase component, IHttpClientFactory cf, NavigationManager nav, IJSRuntime js)
    {
        _component = component;
        HttpClientFactory = cf;
        Navigation = nav;
        JS = js;
        _httpClient = HttpClientFactory.CreateClient("BackendApi");
        PageTitle = "Add New Employee";

        // initialize
        Initialized = true;
        //component.StateHasChanged();
    }

    public HttpClient _httpClient = default!;

    // 2. Fields and properties
    public readonly EmployeeInput employee = new();

    // Notification Fields
    public string? successMessage;
    public string? errorMessage;

    // Loading indicator true = on, false = off
    public bool isLoading = false;

    // 3. Event handlers and public methods
    public async Task HandleValidSubmit()
    {
        // Turn Loading on
        isLoading = true;
        // Clear previous error
        successMessage = errorMessage = null;

        try
        {
            var response = await _httpClient.PostAsJsonAsync("Employee/Create", employee);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<EmployeeViewModel>>();
                var createdEmployee = apiResponse?.Data;
                successMessage = $"New Employee '{createdEmployee?.Name ?? employee.Name}' with id: {createdEmployee?.Id} was added successfully.";
            }
            else
            {
                errorMessage = await response.GetErrorMessageAsync($"Failed to add employee '{employee.Name}'. Please check your input and try again.");
            }
        }
        catch (Exception exception)
        {
            errorMessage = $"Error adding '{employee.Name}': {exception.Message}";
            Console.Error.WriteLine($"Error adding '{employee.Name}': {exception.Message}");
        }
        finally
        {
            // Turn Loading off
            isLoading = false;
        }
    }

    public void GoBack()
    {
        Navigation.NavigateTo("/");
    }

}
