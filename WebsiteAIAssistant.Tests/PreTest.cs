using Microsoft.Extensions.DependencyInjection;

namespace WebsiteAIAssistant.Tests
{
    public class LoadAIModel : IRunBeforeTest
    {
        public Action Run => async () =>
        {
            // Arrange
            // Path to load model
            string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");

            await PredictionEngine.LoadModelAsync(modelPath);
        };
    }

    public class LoadAIListModel : IRunBeforeTest
    {
        public Action Run => async () =>
        {
            // Arrange
            // Path to load model
            string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model-CreateModel-List.zip");

            await PredictionEngine.LoadModelAsync(modelPath);
        };
    }

    public class SetAIModelPath : IRunBeforeTest
    {
        public Action Run => async () =>
        {
            // Arrange
            // Path to load model
            string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");
            // Provide the path to the AI model
            PredictionEngine.AIModelLoadFilePath = modelPath;
        };
    }

    public class BuildCreateModelContainer : IRunBeforeTestReturn
    {
        public Action Run => async () =>
        {
            var sp = await BuildContainerAsync();

            this.ReturnValue = sp;
        };

        public object? ReturnValue { get; set; }

        private async Task<IServiceProvider> BuildContainerAsync()
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
            var sp = services.BuildServiceProvider();

            return await Task.FromResult(sp);
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


    public class BuildLoadPredictContainer : IRunBeforeTestReturn
    {
        public Action Run => async () =>
        {
            var sp = await BuildContainerAsync();

            this.ReturnValue = sp;
        };

        public object? ReturnValue { get; set; }

        private async Task<IServiceProvider> BuildContainerAsync()
        {
            // Build DI container for AI Assistant Service
            var settings = new WebsiteAIAssistantSettings
            {
                AIModelLoadFilePath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip"),
                NegativeConfidenceThreshold = 0.70f,
                NegativeLabel = -1f
            };

            var services = new ServiceCollection();
            services.AddSingleton(settings);
            services.AddSingleton<IWebsiteAIAssistantService, WebsiteAIAssistantService>();
            var sp = services.BuildServiceProvider();

            return await Task.FromResult(sp);
        }
    }
}
