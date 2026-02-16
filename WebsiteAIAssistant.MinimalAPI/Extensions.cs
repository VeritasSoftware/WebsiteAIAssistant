
using Microsoft.Extensions.Caching.Memory;

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

            services.AddHostedService<AIModelLoader>();

            services.AddSingleton(assistantSettings);
        }
    }
}
