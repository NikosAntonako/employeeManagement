using Frontend;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient with the Backend
var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
if (string.IsNullOrEmpty(apiBaseUrl))
    throw new InvalidOperationException("ApiBaseUrl is not configured.");

if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var baseUri))
    throw new UriFormatException($"ApiBaseUrl '{apiBaseUrl}' is not a valid absolute URI.");

builder.Services.AddHttpClient("BackendApi", client =>
{
    client.BaseAddress = baseUri;
});

await builder.Build().RunAsync();
