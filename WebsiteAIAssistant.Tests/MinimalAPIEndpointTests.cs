using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;

namespace WebsiteAIAssistant.Tests
{
    public class MinimalAPIEndpointTests : IClassFixture<WebApplicationFactory<SampleWebsite.MinimalAPI.Startup>>
    {
        private readonly HttpClient _httpClient;

        public MinimalAPIEndpointTests(WebApplicationFactory<SampleWebsite.MinimalAPI.Startup> factory)            
        {
            _httpClient = factory.CreateClient();
        }

        [Theory]
        [InlineData("What are the requisites for carbon credits?", 0)]
        [InlineData("What is the colour of a rose?", -1)]
        public async Task ValidatePredictions(string userInput, float expectedResult)
        {
            // Arrange
            // Minimal API url with user input
            var apiUrl = $"https://localhost:7171/ai/{userInput}";

            // Act
            var response = await _httpClient.GetAsync(apiUrl);

            response.EnsureSuccessStatusCode();

            var strResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            var prediction = JsonSerializer.Deserialize<Prediction>(strResponse, options);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, prediction.PredictedLabel);
        }
    }
}
