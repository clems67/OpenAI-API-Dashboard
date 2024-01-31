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
                var key = await _localstorage.GetItemAsync<string>("OpenAiKey");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", key);
                var result = await httpClient.GetAsync("https://api.openai.com/v1/models");
                if(result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    NavigationManager.NavigateTo("/tuto");
                }
            }
        }
    }
}
