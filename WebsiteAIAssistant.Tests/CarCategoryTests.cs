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

            PredictionEngine.TextFeaturizingEstimatorOptions = new TextFeaturizingEstimatorOptions
            {
                CharFeatureExtractor = new WordBagEstimatorOptions
                {
                    NgramLength = 3, // Only 3-char sequences
                    UseAllLengths = false, // Do not include shorter n-grams  
                    Weighting = WordBagWeightingCriteria.TfIdf
                },
                WordFeatureExtractor = new WordBagEstimatorOptions
                {
                    NgramLength = 3, // Only 3-char sequences
                    UseAllLengths = false, // Do not include shorter n-grams  
                    Weighting = WordBagWeightingCriteria.TfIdf
                }
            };

            string trainingDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "TrainingDataset-CarCategory.tsv");
            PredictionEngine.DataViewFilePath = trainingDataPath;

            string modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model-CarCategory-CreateModel-Test.zip");

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
        [InlineData("price $ 42,000", CarCategory.TwoDoorLuxury)]
        [InlineData("price $ 39,000", CarCategory.TwoDoorBasic)]
        [InlineData("price $ 53,000", CarCategory.TwoDoorLuxury)]
        [InlineData("4 door price $ 67,000", CarCategory.FourDoorBasic)]
        [InlineData("luxury price $ 88,000", CarCategory.FourDoorLuxury)]
        [InlineData("luxury price $ 62,000", CarCategory.TwoDoorLuxury)]
        [InlineData("2 door price $ 29,000", CarCategory.TwoDoorBasic)]
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
