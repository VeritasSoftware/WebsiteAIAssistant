namespace WebsiteAIAssistant.MinimalAPI
{
    public class WebsiteAIAssistantSettings
    {
        public float NegativeConfidenceThreshold { get; set; } = 0.70f;
        public float NegativeLabel { get; set; } = -1f;
        public string AIModelFilePath { get; set; } = string.Empty;
    }
}
