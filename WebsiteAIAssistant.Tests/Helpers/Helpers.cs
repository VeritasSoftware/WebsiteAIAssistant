using Microsoft.Extensions.DependencyInjection;

namespace WebsiteAIAssistant.Tests.Helpers
{
    public static class Helpers
    {
        public static IServiceProvider BuildContainer()
        {
            // Build DI container for AI Assistant Service
            var services = new ServiceCollection();
            services.AddWebsiteAIAssistantCore(settings =>
            {
                settings.AIModelLoadFilePath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model.zip");
                settings.NegativeConfidenceThreshold = 0.70f;
                settings.NegativeLabel = -1f;
            });
            var sp = services.BuildServiceProvider();
            return sp;
        }

        public static IServiceProvider BuildCarCategoryContainer()
        {
            // Build DI container for AI Assistant Service
            var services = new ServiceCollection();
            services.AddWebsiteAIAssistantCore(settings =>
            {
                settings.AIModelLoadFilePath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model-CarCategory.zip");
                settings.NegativeConfidenceThreshold = 0.70f;
                settings.NegativeLabel = -1f;
            });
            var sp = services.BuildServiceProvider();
            return sp;
        }

        public static IServiceProvider BuildCarCategoryMultipleFeatureColumnsFileContainer()
        {
            // Build DI container for AI Assistant Service
            var services = new ServiceCollection();
            services.AddWebsiteAIAssistantCore<ModelInputExtended>(settings =>
            {
                settings.AIModelLoadFilePath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model-CarCategory-MultipleFeatureColumns-File.zip");
                settings.NegativeConfidenceThreshold = 0.70f;
                settings.NegativeLabel = -1f;
            });
            var sp = services.BuildServiceProvider();
            return sp;
        }

        public static IServiceProvider BuildCarCategoryMultipleFeatureColumnsListContainer()
        {
            // Build DI container for AI Assistant Service
            var services = new ServiceCollection();
            services.AddWebsiteAIAssistantCore<ModelInputExtended>(settings =>
            {
                settings.AIModelLoadFilePath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model-CarCategory-MultipleFeatureColumns-List.zip");
                settings.NegativeConfidenceThreshold = 0.70f;
                settings.NegativeLabel = -1f;
            });
            var sp = services.BuildServiceProvider();
            return sp;
        }
    }
}
