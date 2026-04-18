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
                settings.NegativeLabel = "-1";
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
                settings.NegativeLabel = "-1";
            });
            var sp = services.BuildServiceProvider();
            return sp;
        }

        public static IServiceProvider BuildSalesForecastingContainer()
        {
            // Build DI container for AI Assistant Service
            var services = new ServiceCollection();
            services.AddWebsiteAIAssistantCore(settings =>
            {
                settings.AIModelLoadFilePath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model-SalesForecasting.zip");
                settings.NegativeConfidenceThreshold = 0.70f;
                settings.NegativeLabel = "-1";
                settings.ModelType = ModelType.Forecasting;
            });
            var sp = services.BuildServiceProvider();
            return sp;
        }
    }
}
