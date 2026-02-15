
using Microsoft.Extensions.Caching.Memory;

namespace WebsiteAIAssistant.MinimalAPI
{
    public class WebsiteAIAssistantOptions
    {
        public float NegativeConfidenceThreshold { get; set; } = 0.70f;
        public float NegativeLabel { get; set; } = -1f;
        public string AIModelFilePath { get; set; } = string.Empty;
    }

    public static class Extensions
    {
        public static void AddWebsiteAIAssistant(this IServiceCollection services, Action<WebsiteAIAssistantOptions> options)
        {
            var assistantOptions = new WebsiteAIAssistantOptions();
            options(assistantOptions);

            services.AddHostedService<AIModelLoader>();

            services.AddSingleton(assistantOptions);
        }
    }
}
