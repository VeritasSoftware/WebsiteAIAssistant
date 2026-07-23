using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using System.Text.Json;

namespace WebsiteAIAssistant.IntegrationTests
{
    public class MCPServerTests : IAsyncLifetime
    {
        McpClient? _mcpClient;        

        public async Task InitializeAsync()
        {
            var transport = new StdioClientTransport(new()
            {
                Command = "dotnet run",
                Arguments = ["--project", @"..\..\..\..\Sample.WebsiteAIAssistant.MCPServer"],
                Name = "Website AI Assistant MCP Server",
            });
            _mcpClient = await McpClient.CreateAsync(transport);
        }

        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task ValidatePredictions(string userInput, Scheme expectedResult)
        {
            // Arrange
            var param = new CallToolRequestParams
            {
                Name = "get_prediction",
                Arguments = new Dictionary<string, JsonElement>
                {
                    { "input", JsonSerializer.SerializeToElement(userInput) }
                }
            };

            var result = await _mcpClient!.CallToolAsync(param);

            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            var prediction = JsonSerializer.Deserialize<Prediction>(result.Content[0].ToString(), options);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }

        public async Task DisposeAsync()
        {
            if (_mcpClient != null)
            {
                await _mcpClient.DisposeAsync();
            }
        }
    }
}
