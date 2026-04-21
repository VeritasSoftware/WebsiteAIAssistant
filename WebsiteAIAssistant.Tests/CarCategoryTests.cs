using Microsoft.Extensions.DependencyInjection;
using WebsiteAIAssistant.Tests.Helpers;
using xUnitAddons;

namespace WebsiteAIAssistant.Tests
{
    public class CarCategoryTests
    {
        private static IServiceProvider? _aiAssistantServiceProvider;
        private static IServiceProvider? _createModelServiceProvider;
        private static object _lock = new object();
        
        [Fact]
        [Trait("Category", "Create")]
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

        [Fact]
        [Trait("Category", "Create")]
        public async Task CreateModel_CarCategory_MultipleFeatureColumns_File()
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

            // Additional configuration for multiple feature columns
            PredictionEngine.ExtendedColumnNames = new[] { $"{nameof(ModelInputExtended.Feature1)}",
                                                            $"{nameof(ModelInputExtended.Feature2)}",
                                                            $"{nameof(ModelInputExtended.Feature3)}"};


            string trainingDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "TrainingDataset-CarCategory-MultipleFeatureColumns.tsv");
            PredictionEngine.DataViewFilePath = trainingDataPath;

            string modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model-CarCategory-CreateModel-MultipleFeatureColumns-Test.zip");

            if (File.Exists(modelPath))
            {
                File.Delete(modelPath);
            }

            // Act
            await PredictionEngine.CreateModelAsync<ModelInputExtended>(modelPath);

            var modelExists = File.Exists(modelPath);
            // Assert
            Assert.True(modelExists);
        }

        [MyBeforeAfterAsyncTest(typeof(BuildCreateModelMultipleFeatureColumnsContainer), typeof(CarCategoryTests),
                                    $"{nameof(BuildCreateModelDIContainerReturn)}", "dfd72561-e10c-42e9-917b-da049302f031")]
        [Fact]
        [Trait("Category", "Create")]
        public async Task CreateModel_CarCategory_MultipleFeatureColumns_File_Service()
        {
            // Arrange                       
            var createModelSettings = _createModelServiceProvider!.GetRequiredKeyedService<WebsiteAIAssistantCreateModelSettings>("FileSettings");
            var createModelService = _createModelServiceProvider!.GetRequiredKeyedService<IWebsiteAIAssistantCreateModelService>("File");

            // Delete model file if it already exists to ensure a clean test environment
            if (File.Exists(createModelSettings.AIModelFilePath))
            {
                File.Delete(createModelSettings.AIModelFilePath);
            }

            // Act
            var modelCreated = await createModelService.CreateModelAsync<ModelInputExtended>();

            var modelExists = File.Exists(createModelSettings.AIModelFilePath);

            // Assert
            Assert.True(modelCreated);
            Assert.True(modelExists);
        }

        [MyBeforeAfterAsyncTest(typeof(LoadCarCategoryAIModel), typeof(CarCategoryTests),
                            $"{nameof(BuildLoadPredictDIContainerReturn)}", "b9f2641b-d770-47d7-9565-77a64b3df2a4", 10)]
        [Theory]
        [InlineData("price $ 42,000", CarCategory.TwoDoorLuxury)]
        [InlineData("price $ 39,000", CarCategory.TwoDoorBasic)]
        [InlineData("price $ 53,000", CarCategory.TwoDoorLuxury)]
        [InlineData("4 door price $ 67,000", CarCategory.FourDoorBasic)]
        [InlineData("luxury price $ 88,000", CarCategory.FourDoorLuxury)]
        [InlineData("luxury price $ 62,000", CarCategory.TwoDoorLuxury)]
        [InlineData("2 door price $ 29,000", CarCategory.TwoDoorBasic)]
        [InlineData("low price $ 55,000", CarCategory.TwoDoorLuxury)]
        [InlineData("high price $ 34,000", CarCategory.TwoDoorBasic)]
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

        [MyBeforeAfterAsyncTest(typeof(LoadCarCategoryMultipleFeatureColumnsAIModel), typeof(CarCategoryTests),
                            $"{nameof(BuildLoadPredictDIContainerReturn)}", "0887d522-535d-4b79-8f28-d5ede2c3ed76", 2)]
        [Theory]
        [InlineData("2 door", "", "", "$ 42,000", CarCategory.TwoDoorLuxury)]
        [InlineData("", "luxury", "", "$ 100,000", CarCategory.FourDoorLuxury)]
        [InlineData("2 door", "", "", "$ 27,000", CarCategory.TwoDoorBasic)]
        [InlineData("", "basic", "", "$ 75,000", CarCategory.FourDoorBasic)]
        public async Task Load_Predict_Service_CarCategory_MultipleFeatureColumns(string feature, string feature1, string feature2, string feature3, CarCategory expectedResult)
        {
            // Arrange                      
            var aiAssistantService = _aiAssistantServiceProvider!.GetRequiredService<IWebsiteAIAssistantService<ModelInputExtended>>();

            var input = new ModelInputExtended 
            { 
                Feature = feature,
                Feature1 = feature1,
                Feature2 = feature2,
                Feature3 = feature3,
            };

            // Act
            var prediction = await aiAssistantService.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (CarCategory)prediction.PredictedLabel);
        }

        private static void BuildCreateModelDIContainerReturn(object o)
        {
            lock (_lock)
            {
                _createModelServiceProvider = (IServiceProvider)o;
            }
        }

        private static void BuildLoadPredictDIContainerReturn(object o)
        {
            lock(_lock)
            {
                _aiAssistantServiceProvider = (IServiceProvider)o;
            }            
        }
    }
}
