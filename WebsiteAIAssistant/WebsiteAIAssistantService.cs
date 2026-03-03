using System;
using System.IO;
using System.Threading.Tasks;

namespace WebsiteAIAssistant
{
    public class WebsiteAIAssistantService : IWebsiteAIAssistantService
    {
        private readonly WebsiteAIAssistantSettings _settings;

        public WebsiteAIAssistantService(WebsiteAIAssistantSettings settings) 
        { 
            _settings = settings ?? throw new ArgumentNullException(nameof(settings), "Settings cannot be null.");
        }

        public async Task LoadModelAsync()
        {
            if (string.IsNullOrEmpty(_settings.AIModelFilePath))
            {
                throw new InvalidOperationException("AIModelFilePath is null or empty. Please provide a valid file path to the AI model.");
            }
            if (!File.Exists(_settings.AIModelFilePath))
            {
                throw new FileNotFoundException($"AI model file not found at path: {_settings.AIModelFilePath}");
            }
            if (_settings.NegativeConfidenceThreshold < 0 || _settings.NegativeConfidenceThreshold > 1)
            {
                throw new InvalidOperationException("NegativeConfidenceThreshold must be between 0 and 1.");
            }

            PredictionEngine.NegativeConfidenceThreshold = _settings.NegativeConfidenceThreshold;
            PredictionEngine.NegativeLabel = _settings.NegativeLabel;

            await PredictionEngine.LoadModelAsync(_settings.AIModelFilePath);
        }

        public async Task<Prediction> PredictAsync(ModelInput modelInput)
        {
            return await PredictionEngine.PredictAsync(modelInput);
        }
    }
}
