using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using WebsiteAIAssistant.MCPServer.Client;

namespace WebsiteAIAssistant.IntegrationTests
{
    public class MCPClientTests
    {
        IServiceProvider _serviceProvider;

        public MCPClientTests()
        {
            var services = new ServiceCollection();

            services.AddWebsiteAIAssistantMCPClient(settings =>
            {
                settings.ServerBaseUrl = "http://localhost:5000";
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task ValidatePredictions(string userInput, Scheme expectedResult)
        {
            // Arrange
            var mcpClient = _serviceProvider.GetRequiredService<IMCPClient>();

            // Act
            var result = await mcpClient.PostAsync(1, userInput);

            var jsonElement = JsonElement.Parse(result!.Result.Content[0].Text!.ToString());
            var prediction = jsonElement.Deserialize<Prediction>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }
    }
}
