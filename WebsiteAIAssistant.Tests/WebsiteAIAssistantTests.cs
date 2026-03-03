namespace WebsiteAIAssistant.Tests
{
    public class WebsiteAIAssistantTests
    {
        [Fact]
        public async Task CreateModel()
        {
            // Arrange
            PredictionEngine.DataViewType = DataViewType.File;

            string trainingDataPath = Path.Combine(Environment.CurrentDirectory, "TrainingDataset.tsv");
            PredictionEngine.DataViewFilePath = trainingDataPath;
            
            string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model-CreateModel-Test.zip");

            if (File.Exists(modelPath))
            {
                File.Delete(modelPath);
            }

            // Act
            await PredictionEngine.CreateModelAsync(modelPath);

            var modelExists = File.Exists(modelPath);
            // Assert
            Assert.True(modelExists);
        }

        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task ValidatePredictions(string userInput, Scheme expectedResult)
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
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }
    }
}
