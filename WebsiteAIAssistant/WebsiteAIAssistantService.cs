using Microsoft.Extensions.ML;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteAIAssistant
{
    public class WebsiteAIAssistantService : IWebsiteAIAssistantService
    {
        private readonly WebsiteAIAssistantSettings _settings;
        public readonly PredictionEnginePool<ModelInput, Prediction> _predictionEngine;
        private readonly IWebsiteAIAssistantLogger _logger;
        private readonly string _modelKey = $"{nameof(ModelInput)}";
        private readonly float _negativeConfidenceThreshold;
        private readonly float _negativeLabel;

        public WebsiteAIAssistantService(WebsiteAIAssistantSettings settings, PredictionEnginePool<ModelInput, Prediction> predictionEnginePool, IWebsiteAIAssistantLogger logger = null) 
        { 
            _settings = settings ?? throw new ArgumentNullException(nameof(settings), "Settings cannot be null.");
            _predictionEngine = predictionEnginePool ?? throw new ArgumentNullException(nameof(predictionEnginePool), "PredictionEnginePool cannot be null.");  
            _logger = logger;
            _negativeConfidenceThreshold = _settings.NegativeConfidenceThreshold;
            _negativeLabel = _settings.NegativeLabel;
        }

        public bool IsPredictionEngineInitialized => _predictionEngine.GetPredictionEngine(_modelKey) != null;
        
        public async Task<bool> UnloadModelAsync()
        {
            _logger?.LogInformation("Unloading AI model.");
            _predictionEngine.GetPredictionEngine(_modelKey).Dispose();
            _logger?.LogInformation("AI model unloaded successfully.");

            return await Task.FromResult(true);
        }

        public async Task<Prediction> PredictAsync(ModelInput modelInput)
        {     
            _logger?.LogInformation("Making prediction for input: " + modelInput.Feature);
            var prediction = _predictionEngine.Predict(_modelKey, modelInput);

            if (prediction.PredictedLabel <= _negativeLabel)
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

                    prediction.PredictedLabel = index - 1;
                }
                _logger?.LogInformation("Final prediction after confidence check: PredictedLabel={0}", prediction.PredictedLabel);
            }

            _logger?.LogInformation($"Prediction made. PredictedLabel: {prediction.PredictedLabel}, Score: {string.Join(", ", prediction.Score)}");

            return await Task.FromResult(prediction);
        }
    }

    public class WebsiteAIAssistantService<TModelInput> : IWebsiteAIAssistantService<TModelInput>
        where TModelInput : ModelInput
    {
        private readonly WebsiteAIAssistantSettings _settings;
        public readonly PredictionEnginePool<TModelInput, Prediction> _predictionEngine;
        private readonly IWebsiteAIAssistantLogger _logger;
        private readonly string _modelKey = $"{nameof(TModelInput)}";
        private readonly float _negativeConfidenceThreshold;
        private readonly float _negativeLabel;

        public WebsiteAIAssistantService(WebsiteAIAssistantSettings settings, PredictionEnginePool<TModelInput, Prediction> predictionEnginePool, IWebsiteAIAssistantLogger logger = null)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings), "Settings cannot be null.");
            _predictionEngine = predictionEnginePool ?? throw new ArgumentNullException(nameof(predictionEnginePool), "PredictionEnginePool cannot be null.");
            _logger = logger;
            _negativeConfidenceThreshold = _settings.NegativeConfidenceThreshold;
            _negativeLabel = _settings.NegativeLabel;
        }

        public bool IsPredictionEngineInitialized => _predictionEngine.GetPredictionEngine(_modelKey) != null;

        public async Task<bool> UnloadModelAsync()
        {
            _logger?.LogInformation("Unloading AI model.");
            _predictionEngine.GetPredictionEngine(_modelKey).Dispose();
            _logger?.LogInformation("AI model unloaded successfully.");

            return await Task.FromResult(true);
        }

        public async Task<Prediction> PredictAsync(TModelInput modelInput)
        {
            _logger?.LogInformation("Making prediction for input: " + modelInput.Feature);
            var prediction = _predictionEngine.Predict(_modelKey, modelInput);

            if (prediction.PredictedLabel <= _negativeLabel)
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

                    prediction.PredictedLabel = index - 1;
                }
                _logger?.LogInformation("Final prediction after confidence check: PredictedLabel={0}", prediction.PredictedLabel);
            }

            _logger?.LogInformation($"Prediction made. PredictedLabel: {prediction.PredictedLabel}, Score: {string.Join(", ", prediction.Score)}");

            return await Task.FromResult(prediction);
        }
    }
}
