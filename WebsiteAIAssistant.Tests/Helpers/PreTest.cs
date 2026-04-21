using Microsoft.Extensions.DependencyInjection;
using xUnitAddons;

namespace WebsiteAIAssistant.Tests.Helpers
{
    public class LoadAIModel : IRunBeforeAsync, IRunAfterAsync
    {
        public Action RunBefore => async () =>
        {
            // Arrange
            await PredictionEngine.ResetAsync();
            // Path to load model
            string modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model.zip");

            await PredictionEngine.LoadModelAsync(modelPath);
        };

        public Action RunAfter => async () =>
        {
            // Clean up resources after the test, if necessary
             await PredictionEngine.UnloadModelAsync();
        };
    }

    public class LoadCarCategoryAIModel : IRunBeforeAsyncWithReturn
    {
        public Action RunBefore => async () =>
        {
            var sp = await BuildContainerAsync();

            this.ReturnValue = sp;
        };

        public object? ReturnValue { get; set; }

        private async Task<IServiceProvider> BuildContainerAsync()
        {
            // Build DI container for AI Assistant Service
            var sp = Helpers.BuildCarCategoryContainer();

            return await Task.FromResult(sp);
        }
    }

    public class LoadCarCategoryMultipleFeatureColumnsAIModel : IRunBeforeAsyncWithReturn
    {
        public Action RunBefore => async () =>
        {
            var sp = await BuildContainerAsync();

            this.ReturnValue = sp;
        };

        public object? ReturnValue { get; set; }

        private async Task<IServiceProvider> BuildContainerAsync()
        {
            // Build DI container for AI Assistant Service
            var sp = Helpers.BuildCarCategoryMultipleFeatureColumnsContainer();

            return await Task.FromResult(sp);
        }
    }

    public class LoadAIListModel : IRunBeforeAsync, IRunAfterAsync
    {
        public Action RunBefore => async () =>
        {
            // Arrange
            // Path to load model
            string modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model-CreateModel-List.zip");

            await PredictionEngine.LoadModelAsync(modelPath);
        };

        public Action RunAfter => async () =>
        {
            // Clean up resources after the test, if necessary
            await PredictionEngine.UnloadModelAsync();
        };
    }

    public class SetAIModelPath : IRunBeforeAsync, IRunAfterAsync
    {
        public Action RunBefore => async () =>
        {
            // Arrange
            await PredictionEngine.ResetAsync();
            // Path to load model
            string modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model.zip");
            // Provide the path to the AI model
            PredictionEngine.AIModelLoadFilePath = modelPath;
        };

        public Action RunAfter => async () =>
        {
            // Clean up resources after the test, if necessary
            await PredictionEngine.UnloadModelAsync();
        };
    }

    public class BuildCreateModelContainer : IRunBeforeAsyncWithReturn
    {
        public Action RunBefore => async () =>
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
                DataViewFilePath = Path.Combine(Environment.CurrentDirectory, "Data", "TrainingDataset.tsv"),
                AIModelFilePath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model-CreateModel-File-Service-Test.zip")
            };

            var createModelSettingsList = new WebsiteAIAssistantCreateModelSettings
            {
                DataViewType = DataViewType.List,
                DataViewList = LoadListFromFile(Path.Combine(Environment.CurrentDirectory, "Data", "TrainingDataset.tsv")),
                AIModelFilePath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model-CreateModel-List-Service-Test.zip")
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

    public class BuildCreateModelMultipleFeatureColumnsContainer : IRunBeforeAsyncWithReturn
    {
        public Action RunBefore => async () =>
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
                DataViewFilePath = Path.Combine(Environment.CurrentDirectory, "Data", "TrainingDataset-CarCategory-MultipleFeatureColumns.tsv"),
                AIModelFilePath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model-CreateModel-MultipleFeatureColumns-File-Service-Test.zip"),
                // Additional configuration for multiple feature columns
                ExtendedFeatureColumnNames = new[] { $"{nameof(ModelInputExtended.Feature1)}",
                                                        $"{nameof(ModelInputExtended.Feature2)}",
                                                        $"{nameof(ModelInputExtended.Feature3)}"}
            };

            services = new ServiceCollection();
            services.AddKeyedSingleton("FileSettings", createModelSettingsFile);
            services.AddKeyedSingleton<IWebsiteAIAssistantCreateModelService, WebsiteAIAssistantCreateModelService>("File", (sp, x) => new WebsiteAIAssistantCreateModelService(createModelSettingsFile));
            var sp = services.BuildServiceProvider();

            return await Task.FromResult(sp);
        }
    }


    public class BuildLoadPredictContainer : IRunBeforeAsyncWithReturn
    {
        public Action RunBefore => async () =>
        {
            var sp = await BuildContainerAsync();

            this.ReturnValue = sp;
        };

        public object? ReturnValue { get; set; }

        private async Task<IServiceProvider> BuildContainerAsync()
        {
            // Build DI container for AI Assistant Service
            var sp = Helpers.BuildContainer();

            return await Task.FromResult(sp);
        }
    }
}
