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
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task ValidatePredictions(string userInput, Scheme expectedResult)
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
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }

        [Fact]
        public async Task EmptyInput_NotFound()
        {
            // Arrange
            // Minimal API url with user input
            var userInput = "";
            var apiUrl = $"https://localhost:7171/ai/{userInput}";

            // Act
            var response = await _httpClient.GetAsync(apiUrl);

            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task WhitespaceInput_NotFound()
        {
            // Arrange
            // Minimal API url with user input
            var userInput = " ";
            var apiUrl = $"https://localhost:7171/ai/{userInput}";

            // Act
            var response = await _httpClient.GetAsync(apiUrl);

            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
