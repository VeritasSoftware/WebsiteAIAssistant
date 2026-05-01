using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteAIAssistant
{
    public class WebsiteAIAssistantService : IWebsiteAIAssistantService
    {
        private readonly WebsiteAIAssistantSettings _settings;
        private readonly IMyPredictionEnginePool _predictionEnginePool;
        private readonly IWebsiteAIAssistantLogger _logger;
        private readonly float _negativeConfidenceThreshold;
        private readonly float _negativeLabel;

        public WebsiteAIAssistantService(WebsiteAIAssistantSettings settings, IMyPredictionEnginePool predictionEnginePool, IWebsiteAIAssistantLogger logger = null) 
        { 
            _settings = settings ?? throw new ArgumentNullException(nameof(settings), "Settings cannot be null.");
            _predictionEnginePool = predictionEnginePool ?? throw new ArgumentNullException(nameof(predictionEnginePool), "PredictionEnginePool cannot be null.");
            _logger = logger;
            _negativeConfidenceThreshold = _settings.NegativeConfidenceThreshold;
            _negativeLabel = _settings.NegativeLabel;
        }
        
        public async Task<bool> UnloadModelAsync()
        {
            _logger?.LogInformation("Unloading AI model.");
            var modelKey = $"{typeof(ModelInput).Name}";
            _predictionEnginePool.GetPredictionEnginePool<ModelInput>().GetPredictionEngine(modelKey).Dispose();
            _logger?.LogInformation("AI model unloaded successfully.");
            return await Task.FromResult(true);
        }

        public async Task<bool> UnloadModelAsync<TModelInput>()
            where TModelInput : ModelInput
        {
            _logger?.LogInformation("Unloading AI model.");
            var modelKey = $"{typeof(TModelInput).Name}";
            _predictionEnginePool.GetPredictionEnginePool<TModelInput>().GetPredictionEngine(modelKey).Dispose();
            _logger?.LogInformation("AI model unloaded successfully.");

            return await Task.FromResult(true);
        }

        public async Task<Prediction> PredictAsync(ModelInput modelInput)
        {
            return await PredictAsync<ModelInput>(modelInput);
        }

        public async Task<Prediction> PredictAsync<TModelInput>(TModelInput modelInput)
            where TModelInput : ModelInput
        {     
            _logger?.LogInformation("Making prediction for input: " + modelInput.Feature);
            var modelKey = $"{typeof(TModelInput).Name}";
            var prediction = _predictionEnginePool.GetPredictionEnginePool<TModelInput>().GetPredictionEngine(modelKey).Predict(modelInput);

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
