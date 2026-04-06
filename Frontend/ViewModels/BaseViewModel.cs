using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Frontend.ViewModels
{
    public class BaseViewModel
    {
        protected ComponentBase _component;
        protected IHttpClientFactory HttpClientFactory { get; set; } = default!;
        protected NavigationManager Navigation { get; set; } = default!;
        protected IJSRuntime JS { get; set; } = default!;

        public bool Initialized { get; set; }
        public string PageTitle { get; set; }
        public bool IsDevice { get; set; } = false;
    }
}
