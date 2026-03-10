using WebsiteAIAssistant;

namespace SampleWebsite.AWSLambda
{
    public class WebsiteAIAssistantLogger : IWebsiteAIAssistantLogger
    {
        private readonly ILogger<WebsiteAIAssistantLogger> _logger;

        public WebsiteAIAssistantLogger(ILogger<WebsiteAIAssistantLogger> logger)
        {
            _logger = logger;
        }

        public void LogError(string message, params object[] args)
        {
            _logger.LogError(message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }
    }
}
