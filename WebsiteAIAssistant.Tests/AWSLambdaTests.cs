using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using WebsiteAIAssistant.AWSLambda;

namespace WebsiteAIAssistant.Tests
{
    public class AWSLambdaTests
    {
        private readonly static TestLambdaContext testContext = new TestLambdaContext();
        private readonly static Function aiAssistant = new Function();
        private readonly static Func<string, ILambdaContext, Task<object>> aiAssistantLambdaHandler = aiAssistant.GetHandler;

        public AWSLambdaTests()
        {            
        }

        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task AutoLoad_Predict(string userInput, Scheme expectedResult)
        {
            // Path to load model
            string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");

            // Provide the path to the AI model
            PredictionEngine.AIModelLoadFilePath = modelPath;

            // Arrange            
            var input = new ModelInput { Feature = userInput };

            // Act
            var response = await aiAssistantLambdaHandler(userInput, testContext);
            var prediction = response as Prediction;

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }
    }
}
