using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteAIAssistant
{
    public class WebsiteAIAssistantForecastingService : IWebsiteAIAssistantForecastingService
    {
        private readonly WebsiteAIAssistantSettings _settings;
        public readonly PredictionEnginePool<ModelInput, ForecastingPrediction> _predictionEngine;
        private readonly IWebsiteAIAssistantLogger _logger;
        private static bool _isInitialized = false;
        private readonly string _modelKey = $"{nameof(ModelInput)}";
        private readonly float _negativeConfidenceThreshold;
        private readonly string _negativeLabel;

        public WebsiteAIAssistantForecastingService(WebsiteAIAssistantSettings settings, PredictionEnginePool<ModelInput, ForecastingPrediction> predictionEnginePool, IWebsiteAIAssistantLogger logger = null)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings), "Settings cannot be null.");
            _predictionEngine = predictionEnginePool ?? throw new ArgumentNullException(nameof(predictionEnginePool), "PredictionEnginePool cannot be null.");
            _logger = logger;
            _negativeConfidenceThreshold = _settings.NegativeConfidenceThreshold;
            _negativeLabel = _settings.NegativeLabel;
        }

        public bool IsPredictionEngineInitialized => _predictionEngine.GetPredictionEngine(_modelKey) != null;

        [Obsolete("LoadModelAsync is obsolete. The model is now loaded automatically when the PredictionEnginePool is accessed. It will be removed in a future version.")]
        public Task<bool> LoadModelAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ForecastingPrediction> PredictAsync(ModelInput modelInput)
        {
            _logger?.LogInformation("Making prediction for input: " + modelInput.Feature);
            var prediction = _predictionEngine.Predict(_modelKey, modelInput);

            _logger?.LogInformation($"Prediction made. PredictedLabel: {prediction.PredictedLabel}.");

            return await Task.FromResult(prediction);
        }

        public Task<bool> UnloadModelAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class WebsiteAIAssistantService : IWebsiteAIAssistantService
    {
        private readonly WebsiteAIAssistantSettings _settings;
        public readonly PredictionEnginePool<ModelInput, Prediction> _predictionEngine;
        private readonly IWebsiteAIAssistantLogger _logger;
        private static bool _isInitialized = false;
        private readonly string _modelKey = $"{nameof(ModelInput)}";
        private readonly float _negativeConfidenceThreshold;
        private readonly string _negativeLabel;

        public WebsiteAIAssistantService(WebsiteAIAssistantSettings settings, PredictionEnginePool<ModelInput, Prediction> predictionEnginePool, IWebsiteAIAssistantLogger logger = null) 
        { 
            _settings = settings ?? throw new ArgumentNullException(nameof(settings), "Settings cannot be null.");
            _predictionEngine = predictionEnginePool ?? throw new ArgumentNullException(nameof(predictionEnginePool), "PredictionEnginePool cannot be null.");  
            _logger = logger;
            _negativeConfidenceThreshold = _settings.NegativeConfidenceThreshold;
            _negativeLabel = _settings.NegativeLabel;
        }

        public bool IsPredictionEngineInitialized => _predictionEngine.GetPredictionEngine(_modelKey) != null;

        [Obsolete("LoadModelAsync is obsolete. The model is now loaded automatically when the PredictionEnginePool is accessed. It will be removed in a future version.")]
        public async Task<bool> LoadModelAsync()
        {
            if (string.IsNullOrEmpty(_settings.AIModelLoadFilePath))
            {
                _logger?.LogError($"{nameof(_settings.AIModelLoadFilePath)} is null or empty.");
                throw new InvalidOperationException($"{nameof(_settings.AIModelLoadFilePath)} is null or empty. Please provide a valid file path to the AI model.");
            }
            if (!File.Exists(_settings.AIModelLoadFilePath))
            {
                _logger?.LogError($"AI model file not found at path: {_settings.AIModelLoadFilePath}");
                throw new FileNotFoundException($"AI model file not found at path: {_settings.AIModelLoadFilePath}");
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

            _logger?.LogInformation("Loading AI model from file: " + _settings.AIModelLoadFilePath);
            PredictionEngine.NegativeConfidenceThreshold = _settings.NegativeConfidenceThreshold;
            PredictionEngine.NegativeLabel = _settings.NegativeLabel;
            PredictionEngine.AIModelLoadFilePath = _settings.AIModelLoadFilePath;
            PredictionEngine.Logger = _logger;

            _isInitialized = _predictionEngine.GetPredictionEngine(_modelKey) != null;

            _logger?.LogInformation("AI model loaded successfully.");

            return _isInitialized;
        }
        
        public async Task<bool> UnloadModelAsync()
        {
            _logger?.LogInformation("Unloading AI model.");
            _predictionEngine.GetPredictionEngine(_modelKey).Dispose();
            _isInitialized = false;
            _logger?.LogInformation("AI model unloaded successfully.");

            return await Task.FromResult(true);
        }

        public async Task<Prediction> PredictAsync(ModelInput modelInput)
        {     
            _logger?.LogInformation("Making prediction for input: " + modelInput.Feature);
            var prediction = _predictionEngine.Predict(_modelKey, modelInput);

            if (float.Parse(prediction.PredictedLabel) <= float.Parse(_negativeLabel))
            {
                _logger?.LogInformation("Negative prediction detected (PredictedLabel={0}). Checking confidence score...", prediction.PredictedLabel);
                if (prediction.Score[0] < _negativeConfidenceThreshold)
                {
                    _logger?.LogInformation("Negative prediction with low confidence (Score[0]={0}). Checking for second highest score...", prediction.Score[0]);
                    var secondHighestScore = prediction.Score
                                                        .OrderByDescending(s => s)
                                                        .Skip(1)
                                                        .First();
                    var index = Array.IndexOf(prediction.Score, secondHighestScore);

                    prediction.PredictedLabel = (index - 1).ToString();
                }
                _logger?.LogInformation("Final prediction after confidence check: PredictedLabel={0}", prediction.PredictedLabel);
            }

            _logger?.LogInformation($"Prediction made. PredictedLabel: {prediction.PredictedLabel}, Score: {string.Join(", ", prediction.Score)}");

            return await Task.FromResult(prediction);
        }
    }
}
