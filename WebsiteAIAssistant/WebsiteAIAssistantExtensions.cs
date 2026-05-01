using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;
using System;
using System.IO;

namespace WebsiteAIAssistant
{
    public static class WebsiteAIAssistantExtensions
    {
        public static IServiceCollection AddWebsiteAIAssistantCore(this IServiceCollection services, Action<WebsiteAIAssistantSettings> settings)
        {
            services.AddSingleton<IWebsiteAIAssistantService, WebsiteAIAssistantService>();
            services.AddSingleton<IMyPredictionEnginePool, MyPredictionEnginePool>();

            var mySettings = AddWebsiteAIAssistantCoreInternal<ModelInput>(services, settings);

            // Register PredictionEnginePool with a pre-trained model
            services.AddPredictionEnginePool<ModelInput, Prediction>()
                .FromFile(modelName: $"{typeof(ModelInput).Name}", filePath: mySettings.AIModelLoadFilePath, watchForChanges: true);

            return services;
        }

        public static IServiceCollection AddWebsiteAIAssistantCore<TModelInput>(this IServiceCollection services, Action<WebsiteAIAssistantSettings> settings)
            where TModelInput : ModelInput
        {
            services.AddSingleton<IWebsiteAIAssistantService, WebsiteAIAssistantService>();
            services.AddSingleton<IMyPredictionEnginePool, MyPredictionEnginePool>();

            var mySettings = AddWebsiteAIAssistantCoreInternal<TModelInput>(services, settings);

            // Register PredictionEnginePool with a pre-trained model
            services.AddPredictionEnginePool<TModelInput, Prediction>()
                .FromFile(modelName: $"{typeof(TModelInput).Name}", filePath: mySettings.AIModelLoadFilePath, watchForChanges: true);

            return services;
        }

        private static WebsiteAIAssistantSettings AddWebsiteAIAssistantCoreInternal<TModelInput>(this IServiceCollection services, Action<WebsiteAIAssistantSettings> settings)
            where TModelInput : ModelInput
        {
            var assistantSettings = new WebsiteAIAssistantSettings();
            settings(assistantSettings);

            if (string.IsNullOrEmpty(assistantSettings.AIModelLoadFilePath))
            {
                throw new ArgumentException($"{nameof(assistantSettings.AIModelLoadFilePath)} must be provided in the settings.");
            }

            if (!File.Exists(assistantSettings.AIModelLoadFilePath))
            {
                throw new FileNotFoundException($"AI model file not found at path: {assistantSettings.AIModelLoadFilePath}");
            }

            if (assistantSettings.NegativeConfidenceThreshold < 0 || assistantSettings.NegativeConfidenceThreshold > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(assistantSettings.NegativeConfidenceThreshold), $"{nameof(assistantSettings.NegativeConfidenceThreshold)} must be between 0 and 1.");
            }

            services.AddSingleton(assistantSettings);                       

            return assistantSettings;
        }
    }
}
