using Microsoft.Extensions.DependencyInjection;
using WebsiteAIAssistant.Tests.Helpers;
using xUnitAddons;

namespace WebsiteAIAssistant.Tests
{
    public class WebsiteAIAssistantTests
    {
        private static IServiceProvider? _aiAssistantServiceProvider;
        private static IServiceProvider? _createModelServiceProvider;

        [Fact]
        [Trait("Category", "Create")]
        public async Task CreateModel_File()
        {
            // Arrange
            PredictionEngine.DataViewType = DataViewType.File;

            string trainingDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "TrainingDataset.tsv");
            PredictionEngine.DataViewFilePath = trainingDataPath;

            string modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model-CreateModel-File-Test.zip");

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
        public async Task CreateModel_List()
        {
            // Arrange
            PredictionEngine.DataViewType = DataViewType.List;

            string trainingDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "TrainingDataset.tsv");

            PredictionEngine.DataViewList = LoadListFromFile(trainingDataPath);

            string modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model-CreateModel-List-Test.zip");
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

        [MyBeforeAfterAsyncTest(typeof(BuildCreateModelContainer), typeof(WebsiteAIAssistantTests), 
                                    $"{nameof(BuildCreateModelDIContainerReturn)}", "5bffdd98-b7e9-436d-9a92-beb7b6801975")]
        [Fact]
        [Trait("Category", "Create")]
        public async Task CreateModel_File_Service()
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
            var modelCreated = await createModelService.CreateModelAsync();

            var modelExists = File.Exists(createModelSettings.AIModelFilePath);

            // Assert
            Assert.True(modelCreated);
            Assert.True(modelExists);
        }

        [MyBeforeAfterAsyncTest(typeof(BuildCreateModelContainer), typeof(WebsiteAIAssistantTests),
                                    $"{nameof(BuildCreateModelDIContainerReturn)}", "49027756-c399-498c-8c2f-f82e5392882c")]
        [Fact]
        [Trait("Category", "Create")]
        public async Task CreateModel_List_Service()
        {
            // Arrange                       
            var createModelSettings = _createModelServiceProvider!.GetRequiredKeyedService<WebsiteAIAssistantCreateModelSettings>("ListSettings");
            var createModelService = _createModelServiceProvider!.GetRequiredKeyedService<IWebsiteAIAssistantCreateModelService>("List");

            // Delete model file if it already exists to ensure a clean test environment
            if (File.Exists(createModelSettings.AIModelFilePath))
            {
                File.Delete(createModelSettings.AIModelFilePath);
            }

            // Act
            var modelCreated = await createModelService.CreateModelAsync();

            var modelExists = File.Exists(createModelSettings.AIModelFilePath);

            // Assert
            Assert.True(modelCreated);
            Assert.True(modelExists);
        }

        [MyBeforeAfterAsyncTest(typeof(LoadAIModel), "5bb02c70-01d1-4987-8a6e-ab7fc8b1dcc4", 3)]
        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task Load_Predict(string userInput, Scheme expectedResult)
        {
            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await PredictionEngine.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }

        [MyBeforeAfterAsyncTest(typeof(SetAIModelPath), "d54e2920-ad42-4acc-a6e2-37aad8e9ac3f", 3)]
        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task AutoLoad_Predict(string userInput, Scheme expectedResult)
        {
            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await PredictionEngine.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }

        [MyBeforeAfterAsyncTest(typeof(LoadAIListModel), "1761b894-e972-4c2f-ab01-1c07b4867bd1", 3)]
        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task Load_Predict_List(string userInput, Scheme expectedResult)
        {
            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await PredictionEngine.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }

        [MyBeforeAfterAsyncTest(typeof(BuildLoadPredictContainer), typeof(WebsiteAIAssistantTests),
                                    $"{nameof(BuildLoadPredictDIContainerReturn)}", "5bb02c70-01d1-4987-8a6e-ab7fc8b1dcc4", 3)]
        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task Load_Predict_Service(string userInput, Scheme expectedResult)
        {
            // Arrange                      
            var aiAssistantService = _aiAssistantServiceProvider!.GetRequiredService<IWebsiteAIAssistantService>();

            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await aiAssistantService.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }

        [MyBeforeAfterAsyncTest(typeof(BuildLoadPredictContainer), typeof(WebsiteAIAssistantTests),
                                    $"{nameof(BuildLoadPredictDIContainerReturn)}", "ec94f239-86b9-4563-8b1d-2e85c65fb9d2", 3)]
        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task AutoLoad_Predict_Service(string userInput, Scheme expectedResult)
        {
            // Arrange                      
            var aiAssistantService = _aiAssistantServiceProvider!.GetRequiredService<IWebsiteAIAssistantService>();

            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await aiAssistantService.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }

        [Fact]
        public async Task UnloadModel()
        {
            // Arrange
            // Path to load model
            string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");

            await PredictionEngine.LoadModelAsync(modelPath);

            var unloaded = await PredictionEngine.UnloadModelAsync();

            // Assert
            Assert.True(unloaded);
        }

        [MyBeforeAfterAsyncTest(typeof(BuildLoadPredictContainer), typeof(WebsiteAIAssistantTests),
                                    $"{nameof(BuildLoadPredictDIContainerReturn)}", "ea1d6f3b-6fc2-462e-af85-eb90014414e8")]
        [Fact]
        public async Task UnloadModel_Service()
        {
            var service = _aiAssistantServiceProvider!.GetRequiredService<IWebsiteAIAssistantService>();

            var unloaded = await service.UnloadModelAsync();
            // Assert
            Assert.True(unloaded);
        }

        private static void BuildLoadPredictDIContainerReturn(object o)
        {
            _aiAssistantServiceProvider = (IServiceProvider)o;
        }

        private static void BuildCreateModelDIContainerReturn(object o)
        {
            _createModelServiceProvider = (IServiceProvider)o;
        }

        private static IEnumerable<ModelInput> LoadListFromFile(string filePath)
        {
            var data = new List<ModelInput>();
            using var reader = new StreamReader(filePath);
            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                var parts = line.Split('\t');
                if (parts.Length == 2 && float.TryParse(parts[0], out float label))
                {
                    data.Add(new ModelInput
                    {
                        Label = label,
                        Feature = parts[1]
                    });
                }
            }
            return data;
        }
    }
}
