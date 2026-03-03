namespace WebsiteAIAssistant.MinimalAPI
{
    public static class Extensions
    {
        public static void AddWebsiteAIAssistant(this IServiceCollection services, Action<WebsiteAIAssistantSettings> settings)
        {
            var assistantSettings = new WebsiteAIAssistantSettings();
            settings(assistantSettings);

            if (string.IsNullOrEmpty(assistantSettings.AIModelFilePath))
            {
                throw new ArgumentException("AIModelFilePath must be provided in the settings.");
            }

            if (!File.Exists(assistantSettings.AIModelFilePath))
            {
                throw new FileNotFoundException($"AI model file not found at path: {assistantSettings.AIModelFilePath}");
            }

            if (assistantSettings.NegativeConfidenceThreshold < 0 || assistantSettings.NegativeConfidenceThreshold > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(assistantSettings.NegativeConfidenceThreshold), "NegativeConfidenceThreshold must be between 0 and 1.");
            }

            services.AddSingleton(assistantSettings);

            services.AddSingleton<IWebsiteAIAssistantService, WebsiteAIAssistantService>();

            services.AddHostedService<AIModelLoader>();            
        }
    }
}
