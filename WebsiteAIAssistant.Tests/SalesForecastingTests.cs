using Microsoft.Extensions.DependencyInjection;
using WebsiteAIAssistant.Tests.Helpers;
using xUnitAddons;

namespace WebsiteAIAssistant.Tests
{
    public class SalesForecastingTests
    {
        private static IServiceProvider? _aiAssistantServiceProvider;

        [Fact]
        public async Task CreateModel_SalesForecasting_File()
        {
            // Arrange
            PredictionEngine.DataViewType = DataViewType.File;
            PredictionEngine.ModelType = ModelType.Forecasting;

            string trainingDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "TrainingDataset-SalesForecasting.tsv");
            PredictionEngine.DataViewFilePath = trainingDataPath;

            string modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model-SalesForecasting-CreateModel-Test.zip");

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

        [MyBeforeAfterAsyncTest(typeof(LoadSalesForecastingAIModel), typeof(SalesForecastingTests),
                            $"{nameof(BuildLoadPredictDIContainerReturn)}", "b9f2641b-d770-47d7-9565-77a64b3df2a4", 10)]
        [Theory]
        [InlineData("2026-01", 3043.32666f)]
        [InlineData("2026-06", 3996.605f)]
        [InlineData("2026-12", 4542.681f)]
        public async Task Load_Predict_Service_SalesForecasting(string userInput, float expectedResult)
        {
            // Arrange                      
            var aiAssistantService = _aiAssistantServiceProvider!.GetRequiredService<IWebsiteAIAssistantForecastingService>();

            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await aiAssistantService.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult,prediction.PredictedLabel);
        }

        private static void BuildLoadPredictDIContainerReturn(object o)
        {
            _aiAssistantServiceProvider = (IServiceProvider)o;
        }
    }
}
