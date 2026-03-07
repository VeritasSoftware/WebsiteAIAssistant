using Microsoft.Extensions.DependencyInjection;

namespace WebsiteAIAssistant.Tests
{
    public class WebsiteAIAssistantTests
    {
        private readonly IServiceProvider _aiAssistantServiceProvider;
        private readonly IServiceProvider _createModelServiceProvider;

        public WebsiteAIAssistantTests()
        {
            // Build DI container for Create Model Service            
            _createModelServiceProvider = BuildCreateModelDIContainer();

            // Build DI container for AI Assistant Service
            _aiAssistantServiceProvider = BuildLoadPredictDIContainer();
        }

        [Fact]
        public async Task CreateModel_File()
        {
            // Arrange
            PredictionEngine.DataViewType = DataViewType.File;

            string trainingDataPath = Path.Combine(Environment.CurrentDirectory, "TrainingDataset.tsv");
            PredictionEngine.DataViewFilePath = trainingDataPath;
            
            string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model-CreateModel-File-Test.zip");

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
        public async Task CreateModel_List()
        {
            // Arrange
            PredictionEngine.DataViewType = DataViewType.List;

            string trainingDataPath = Path.Combine(Environment.CurrentDirectory, "TrainingDataset.tsv");

            PredictionEngine.DataViewList = LoadListFromFile(trainingDataPath);

            string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model-CreateModel-List-Test.zip");

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
        public async Task CreateModel_File_Service()
        {
            // Arrange                       
            var createModelSettings = _createModelServiceProvider.GetRequiredKeyedService<WebsiteAIAssistantCreateModelSettings>("FileSettings");
            var createModelService = _createModelServiceProvider.GetRequiredKeyedService<IWebsiteAIAssistantCreateModelService>("File");

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

        [Fact]
        public async Task CreateModel_List_Service()
        {
            // Arrange                       
            var createModelSettings = _createModelServiceProvider.GetRequiredKeyedService<WebsiteAIAssistantCreateModelSettings>("ListSettings");
            var createModelService = _createModelServiceProvider.GetRequiredKeyedService<IWebsiteAIAssistantCreateModelService>("List");

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

        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task Load_Predict(string userInput, Scheme expectedResult)
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

        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task Load_Predict_List(string userInput, Scheme expectedResult)
        {
            // Arrange
            // Path to load model created from list
            string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model-CreateModel-List.zip");

            await PredictionEngine.LoadModelAsync(modelPath);

            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await PredictionEngine.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }

        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task Load_Predict_Service(string userInput, Scheme expectedResult)
        {
            // Arrange                      
            var aiAssistantService = _aiAssistantServiceProvider.GetRequiredService<IWebsiteAIAssistantService>();

            await aiAssistantService.LoadModelAsync();

            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await aiAssistantService.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }

        private IServiceProvider BuildCreateModelDIContainer()
        {
            // Build DI container for Create Model Service
            var services = new ServiceCollection();
            var createModelSettingsFile = new WebsiteAIAssistantCreateModelSettings
            {
                DataViewType = DataViewType.File,
                DataViewFilePath = Path.Combine(Environment.CurrentDirectory, "TrainingDataset.tsv"),
                AIModelFilePath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model-CreateModel-File-Service-Test.zip")
            };

            var createModelSettingsList = new WebsiteAIAssistantCreateModelSettings
            {
                DataViewType = DataViewType.List,
                DataViewList = LoadListFromFile(Path.Combine(Environment.CurrentDirectory, "TrainingDataset.tsv")),
                AIModelFilePath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model-CreateModel-List-Service-Test.zip")
            };

            services = new ServiceCollection();
            services.AddKeyedSingleton("FileSettings", createModelSettingsFile);
            services.AddKeyedSingleton("ListSettings", createModelSettingsList);
            services.AddKeyedSingleton<IWebsiteAIAssistantCreateModelService, WebsiteAIAssistantCreateModelService>("File", (sp, x) => new WebsiteAIAssistantCreateModelService(createModelSettingsFile));
            services.AddKeyedSingleton<IWebsiteAIAssistantCreateModelService, WebsiteAIAssistantCreateModelService>("List", (sp, x) => new WebsiteAIAssistantCreateModelService(createModelSettingsList));
            return services.BuildServiceProvider();            
        }

        private IServiceProvider BuildLoadPredictDIContainer()
        {
            // Build DI container for AI Assistant Service
            var settings = new WebsiteAIAssistantSettings
            {
                AIModelFilePath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip"),
                NegativeConfidenceThreshold = 0.70f,
                NegativeLabel = -1f
            };

            var services = new ServiceCollection();
            services.AddSingleton(settings);
            services.AddSingleton<IWebsiteAIAssistantService, WebsiteAIAssistantService>();
            return services.BuildServiceProvider();
        }

        private IEnumerable<ModelInput> LoadListFromFile(string filePath)
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
