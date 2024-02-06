using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace OpenAI_API_Dashboard.Pages
{
    public partial class Index
    {
        [Inject] public NavigationManager NavigationManager { get; set; } = default!;
        private async Task GetLLMsModelsAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var key = await _localstorage.GetItemAsync<string>("OpenAiKey");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", key);
                var result = await httpClient.GetAsync("https://api.openai.com/v1/models");
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    NavigationManager.NavigateTo("/OpenAI-API-Dashboard/tuto");
                }
            }
        }

        private async Task PostMessageAsync(string question)
        {
            using (var httpClient = new HttpClient())
            {

                var data = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new {role = "user", content = question}
                    }
                };

                var jsonData = JsonSerializer.Serialize(data);

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var key = await _localstorage.GetItemAsync<string>("OpenAiKey");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", key);

                var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    NavigationManager.NavigateTo("/tuto");
                    return;
                }

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _openErrorMessage = true;
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiError = JsonSerializer.Deserialize<ApiError>(responseContent);
                    if(apiError.error.type == "insufficient_quota")
                    {
                        errorMessage = "You already spent everything !\nGo check : https://platform.openai.com/usage";
                        return;
                    }
                    errorMessage = apiError.error.message;
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine(responseBody);
            }
        }
        public class ErrorDetails
        {
            public string message { get; set; }
            public string type { get; set; }
            public object param { get; set; }
            public string code { get; set; }
        }
        public class ApiError
        {
            public ErrorDetails error { get; set; }
        }
    }
}
