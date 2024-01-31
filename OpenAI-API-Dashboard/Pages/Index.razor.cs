using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;

namespace OpenAI_API_Dashboard.Pages
{
    public partial class Index
    {
        [Inject] public NavigationManager NavigationManager { get; set; } = default!;
        private async Task GetLLMsModelsAsync()
        {
            using(var httpClient = new HttpClient())
            {
                var result = await httpClient.GetAsync("https://api.openai.com/v1/models");
                if(result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    NavigationManager.NavigateTo("/tuto");
                }
            }
        }
    }
}
