using Microsoft.Learn.AzureFunctionsTesting;
using System.Text.Json;
using WebsiteAIAssistant.Tests.AzureFunction;

[assembly: TestFramework("Microsoft.Learn.AzureFunctionsTesting.TestFramework", "Microsoft.Learn.AzureFunctionsTesting")]
[assembly: AssemblyFixture(typeof(FunctionFixture<FunctionStartup>))]
namespace WebsiteAIAssistant.Tests.AzureFunction
{    
    public class FunctionStartup : IFunctionTestStartup
    {
        public void Configure(FunctionTestConfigurationBuilder builder)
        {
            var solutionDirectoryPath = TryGetSolutionDirectoryInfo(Environment.CurrentDirectory);

            var path = Path.Combine(solutionDirectoryPath!.FullName, @"SampleWebsite.AzureFunction/bin/Debug/net8.0");

            builder.SetFunctionAppPath(path);
        }

        public static DirectoryInfo? TryGetSolutionDirectoryInfo(string? currentPath = null)
        {
            DirectoryInfo? directory = new(
                currentPath ?? Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.slnx").Any())
            {
                directory = directory.Parent;
            }
            return directory;
        }
    }
    
    // Define a collection for the FunctionFixture<FunctionStartup>
    [CollectionDefinition("Function collection")]
    public class FunctionCollection : ICollectionFixture<FunctionFixture<FunctionStartup>>
    {
        // This class has no code, and is never created. Its purpose is just to be the place to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
    }

    [Collection("Function collection")]
    public class AzureFunctionEndpointTests
    {
        private readonly HttpClient _httpClient;

        public AzureFunctionEndpointTests(FunctionFixture<FunctionStartup> fixture)            
        {
            _httpClient = fixture.Client;
        }

        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task ValidatePredictions(string userInput, Scheme expectedResult)
        {
            // Arrange
            // Function url with user input
            var apiUrl = $"/ai/{userInput}";

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
            // Function url with user input
            var userInput = "";
            var apiUrl = $"/ai/{userInput}";

            // Act
            var response = await _httpClient.GetAsync(apiUrl);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task WhitespaceInput_NotFound()
        {
            // Arrange
            // Function url with user input
            var userInput = " ";
            var apiUrl = $"/ai/{userInput}";

            // Act
            var response = await _httpClient.GetAsync(apiUrl);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
