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
        public static void AddWebsiteAIAssistant(this IServiceCollection services, Action<WebsiteAIAssistantOptions>? options = null)
        {
            if (options != null)
            {
                var assistantOptions = new WebsiteAIAssistantOptions();
                options(assistantOptions);

                PredictionEngine.NegativeConfidenceThreshold = assistantOptions.NegativeConfidenceThreshold;
                PredictionEngine.NegativeLabel = assistantOptions.NegativeLabel;

                PredictionEngine.LoadModelAsync(assistantOptions.AIModelFilePath).ConfigureAwait(false).GetAwaiter().GetResult();

                services.AddSingleton(assistantOptions);
            }
            else
            {
                services.AddSingleton(new WebsiteAIAssistantOptions());
            }
        }
    }
}
