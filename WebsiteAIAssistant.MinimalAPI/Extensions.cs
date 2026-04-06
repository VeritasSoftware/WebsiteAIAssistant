namespace WebsiteAIAssistant.MinimalAPI
{
    public static class Extensions
    {
        public static void AddWebsiteAIAssistant(this IServiceCollection services, Action<WebsiteAIAssistantSettings> settings)
        {
            services.AddWebsiteAIAssistantCore(settings);

            services.AddHostedService<AIModelLoader>();            
        }
    }
}
