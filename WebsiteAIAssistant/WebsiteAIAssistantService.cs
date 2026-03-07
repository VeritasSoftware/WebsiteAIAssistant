using System;
using System.IO;
using System.Threading.Tasks;

namespace WebsiteAIAssistant
{
    public class WebsiteAIAssistantService : IWebsiteAIAssistantService
    {
        private readonly WebsiteAIAssistantSettings _settings;
        private readonly IWebsiteAIAssistantLogger _logger;

        public WebsiteAIAssistantService(WebsiteAIAssistantSettings settings, IWebsiteAIAssistantLogger logger = null) 
        { 
            _settings = settings ?? throw new ArgumentNullException(nameof(settings), "Settings cannot be null.");
            _logger = logger;
        }

        public async Task LoadModelAsync()
        {
            if (string.IsNullOrEmpty(_settings.AIModelFilePath))
            {
                _logger?.LogError($"{nameof(_settings.AIModelFilePath)} is null or empty.");
                throw new InvalidOperationException($"{nameof(_settings.AIModelFilePath)} is null or empty. Please provide a valid file path to the AI model.");
            }
            if (!File.Exists(_settings.AIModelFilePath))
            {
                _logger?.LogError($"AI model file not found at path: {_settings.AIModelFilePath}");
                throw new FileNotFoundException($"AI model file not found at path: {_settings.AIModelFilePath}");
            }
            // Validate NegativeConfidenceThreshold
            if (_settings.NegativeConfidenceThreshold >= 0 && _settings.NegativeConfidenceThreshold <= 1)
            {
                if (_settings.NegativeConfidenceThreshold < .50f)
                {
                    _logger?.LogWarning($"{nameof(_settings.NegativeConfidenceThreshold)} : {_settings.NegativeConfidenceThreshold} too low. Not enough confidence to classify as negative prediction.");
                }
                else if (_settings.NegativeConfidenceThreshold > .90f)
                {
                    _logger?.LogWarning($"{nameof(_settings.NegativeConfidenceThreshold)} : {_settings.NegativeConfidenceThreshold} too high. May miss valid negative predictions.");
                }
            }
            else
            {
                _logger?.LogError($"{nameof(_settings.NegativeConfidenceThreshold)} must be between 0 and 1.");
                throw new InvalidOperationException($"{nameof(_settings.NegativeConfidenceThreshold)} must be between 0 and 1.");
            }

            _logger?.LogInformation("Loading AI model from file: " + _settings.AIModelFilePath);

            PredictionEngine.NegativeConfidenceThreshold = _settings.NegativeConfidenceThreshold;
            PredictionEngine.NegativeLabel = _settings.NegativeLabel;
            PredictionEngine.Logger = _logger;

            await PredictionEngine.LoadModelAsync(_settings.AIModelFilePath);

            _logger?.LogInformation("AI model loaded successfully.");
        }

        public async Task<Prediction> PredictAsync(ModelInput modelInput)
        {
            _logger?.LogInformation("Making prediction for input: " + modelInput.Feature);
            var prediction = await PredictionEngine.PredictAsync(modelInput);
            _logger?.LogInformation($"Prediction made. PredictedLabel: {prediction.PredictedLabel}, Score: {string.Join(", ", prediction.Score)}");

            return prediction;
        }
    }
}
