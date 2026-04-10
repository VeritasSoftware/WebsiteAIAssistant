using Microsoft.Extensions.DependencyInjection;
using WebsiteAIAssistant.Tests.Helpers;
using xUnitAddons;

namespace WebsiteAIAssistant.Tests
{
    public class CarCategoryTests
    {
        private static IServiceProvider? _aiAssistantServiceProvider;

        [Fact]
        public async Task CreateModel_CarCategory_File()
        {
            // Arrange
            PredictionEngine.DataViewType = DataViewType.File;

            string trainingDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "TrainingDataset-CarCategory.tsv");
            PredictionEngine.DataViewFilePath = trainingDataPath;

            string modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model-CarCategory.zip");

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

        [MyBeforeAfterAsyncTest(typeof(LoadCarCategoryAIModel), typeof(CarCategoryTests),
                            $"{nameof(BuildLoadPredictDIContainerReturn)}", "b9f2641b-d770-47d7-9565-77a64b3df2a4", 4)]
        [Theory]
        [InlineData("price 47000", CarCategory.TwoDoorTwoEngine)]
        [InlineData("4 door 67000", CarCategory.FourDoorOneEngine)]
        [InlineData("2 engine 87000", CarCategory.FourDoorTwoEngine)]
        [InlineData("2 door 47000", CarCategory.TwoDoorTwoEngine)]
        [InlineData("What is the colour of a rose?", CarCategory.None)]
        public async Task Load_Predict_Service_CarCategory(string userInput, CarCategory expectedResult)
        {
            // Arrange                      
            var aiAssistantService = _aiAssistantServiceProvider!.GetRequiredService<IWebsiteAIAssistantService>();

            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await aiAssistantService.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (CarCategory)prediction.PredictedLabel);
        }

        private static void BuildLoadPredictDIContainerReturn(object o)
        {
            _aiAssistantServiceProvider = (IServiceProvider)o;
        }
    }
}
