using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using WebsiteAIAssistant.AWSLambda;

namespace WebsiteAIAssistant.Tests
{
    public class AWSLambdaTests
    {
        private readonly static TestLambdaContext testContext = new TestLambdaContext();
        private readonly static PredictionLambda aiAssistant = new PredictionLambda();
        private readonly static Func<string, ILambdaContext, Task<object>> aiAssistantLambdaHandler = aiAssistant.GetHandler;

        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task AutoLoad_Predict(string userInput, Scheme expectedResult)
        {
            // Arrange
            // Path to load model
            string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");

            // Provide the path to the AI model
            PredictionEngine.AIModelLoadFilePath = modelPath;                        

            // Act
            var response = await aiAssistantLambdaHandler(userInput, testContext);
            var prediction = response as Prediction;

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }

        [Fact]
        public async Task EmptyInput_Fail()
        {
            // Arrange
            // Path to load model
            string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");

            // Provide the path to the AI model
            PredictionEngine.AIModelLoadFilePath = modelPath;
                        
            var input = string.Empty;

            // Act
            var response = await aiAssistantLambdaHandler(input, testContext);

            Assert.NotNull(response);
            Assert.Equal("Input cannot be empty.", response);
        }
    }
}
