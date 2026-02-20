using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Text.Json;

namespace WebsiteAIAssistant.Tests
{
    public class MinimalAPIInitialize
    {
        public TestServer MinimalAPI { get; set; }

        public MinimalAPIInitialize()
        {
            //Start Gateway API
            IWebHostBuilder minimalAPI = new WebHostBuilder()
                                     .UseStartup<SampleWebsite.MinimalAPI.Startup>()
                                     .UseKestrel(options => options.Listen(IPAddress.Any, 7171, listenOptions => listenOptions.UseHttps(o => o.AllowAnyClientCertificate())));

            TestServer testServer = new TestServer(minimalAPI);

            this.MinimalAPI = testServer;
        }
    }

    public class MinimalAPIEndpointTests : IClassFixture<MinimalAPIInitialize>
    {
        readonly MinimalAPIInitialize _apiInit;

        public MinimalAPIEndpointTests(MinimalAPIInitialize apiInit)            
        {
            _apiInit = apiInit;
        }

        [Theory]
        [InlineData("What are the requisites for carbon credits?", 0)]
        [InlineData("What is the colour of a rose?", -1)]
        public async Task ValidatePredictions(string userInput, float expectedResult)
        {
            // Arrange
            var client = _apiInit.MinimalAPI.CreateClient();

            // Minimal API url with user input
            var apiUrl = $"https://localhost:7171/ai/{userInput}";

            // Act
            var response = await client.GetAsync(apiUrl);

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
