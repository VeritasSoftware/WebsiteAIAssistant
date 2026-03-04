using Microsoft.Extensions.DependencyInjection;

namespace WebsiteAIAssistant.Tests
{
    public class WebsiteAIAssistantTests
    {
        private readonly ServiceProvider _aiAssistantServiceProvider;
        private readonly ServiceProvider _createModelServiceProvider;

        public WebsiteAIAssistantTests()
        {
            // Build DI container for Create Model Service
            var services = new ServiceCollection();
            var createModelSettings = new WebsiteAIAssistantCreateModelSettings
            {
                DataViewType = DataViewType.File,
                DataViewFilePath = Path.Combine(Environment.CurrentDirectory, "TrainingDataset.tsv"),
                AIModelFilePath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model-CreateModel-Service-Test.zip")
            };

            services = new ServiceCollection();
            services.AddSingleton(createModelSettings);
            services.AddSingleton<IWebsiteAIAssistantCreateModelService, WebsiteAIAssistantCreateModelService>();
            _createModelServiceProvider = services.BuildServiceProvider();

            // Build DI container for AI Assistant Service
            var settings = new WebsiteAIAssistantSettings
            {                
                AIModelFilePath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip"),
                NegativeConfidenceThreshold = 0.70f,
                NegativeLabel = -1f
            };

            services = new ServiceCollection();
            services.AddSingleton(settings);
            services.AddSingleton<IWebsiteAIAssistantService, WebsiteAIAssistantService>();
            _aiAssistantServiceProvider = services.BuildServiceProvider();            
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

            using var reader = new StreamReader(trainingDataPath);

            var dataViewList = new List<ModelInput>();

            string? line;

            while ((line = await reader.ReadLineAsync()) is not null)
            {
                var parts = line.Split('\t');
                if (parts.Length == 2 && float.TryParse(parts[0], out float label))
                {
                    dataViewList.Add(new ModelInput
                    {
                        Label = label,
                        Feature = parts[1]
                    });
                }
            }

            PredictionEngine.DataViewList = dataViewList;

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
            var createModelSettings = _createModelServiceProvider.GetRequiredService<WebsiteAIAssistantCreateModelSettings>();
            var createModelService = _createModelServiceProvider.GetRequiredService<IWebsiteAIAssistantCreateModelService>();

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
            string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model-CreateModel-List-Test.zip");

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
    }
}
