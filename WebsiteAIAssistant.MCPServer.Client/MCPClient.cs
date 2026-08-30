using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebsiteAIAssistant.MCPServer.Client
{
    public interface IMCPClient
    {
        Task<ServerResult?> PostAsync(int id, string userInput);
    }

    public class MCPClient : IMCPClient
    {
        private readonly HttpClient _httpClient;

        public event Func<string, Task>? OnResponseReceived;

        public MCPClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MCPClient");
        }

        public async Task<ServerResult?> PostAsync(int id, string userInput)
        {
            var request = new MCPRequest(id, userInput);

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("mcp", content);

            // Ensure success status code
            response.EnsureSuccessStatusCode();

            // Read the response body
            string responseBody = await response.Content.ReadAsStringAsync();

            if (OnResponseReceived != null)
            {
                await OnResponseReceived(responseBody);

                return null;
            }
            else
            {
                var m = Regex.Match(responseBody, ".*?(?<data>\\{.*\\})");

                if (m.Success)
                {
                    var responseStr = m.Groups["data"].Captures[0].Value;

                    var serverResult = JsonSerializer.Deserialize<ServerResult>(responseStr);

                    return serverResult;
                }

                return null;
            }           
        }
    }
}