namespace WebsiteAIAssistant.Tests
{
    public class WebsiteAIAssistantTests
    {
        [Theory]
        [InlineData("What are the requisites for carbon credits?", 0)]
        [InlineData("What is the colour of a rose?", -1)]
        public async Task ValidatePredictions(string userInput, float expectedResult)
        {
            // Arrange
            // Path to load model
            string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");

            await PredictionEngine.LoadModelAsync(modelPath);

            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await PredictionEngine.PredictAsync(input);            

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, prediction.PredictedLabel);
        }
    }
}
