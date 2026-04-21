using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteAIAssistant
{
    public class WebsiteAIAssistantCreateModelService : IWebsiteAIAssistantCreateModelService
    {
        private readonly WebsiteAIAssistantCreateModelSettings _settings;
        private readonly IWebsiteAIAssistantLogger _logger;

        public WebsiteAIAssistantCreateModelService(WebsiteAIAssistantCreateModelSettings settings, IWebsiteAIAssistantLogger logger = null)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger;
        }

        public async Task<bool> CreateModelAsync()
        {
            return await CreateModelAsync<ModelInput>();
        }

        public async Task<bool> CreateModelAsync<TModelInput>()
            where TModelInput : ModelInput, new()
        {
            _logger?.LogInformation("Starting model creation process...");
            _logger?.LogInformation($"Setting {nameof(DataViewType)}: {_settings.DataViewType}");
            PredictionEngine.DataViewType = _settings.DataViewType;

            if (_settings.DataViewType == DataViewType.File)
            {
                if (string.IsNullOrEmpty(_settings.DataViewFilePath))
                {
                    _logger?.LogError($"{nameof(_settings.DataViewFilePath)} is null or empty. Please provide a valid file path to the training data.");
                    throw new InvalidOperationException($"{nameof(_settings.DataViewFilePath)} is null or empty. Please provide a valid file path to the training data.");
                }

                _logger?.LogInformation($"Using data view file path: {_settings.DataViewFilePath}");
                PredictionEngine.DataViewFilePath = _settings.DataViewFilePath;
            }
            else
            {
                if (_settings.DataViewList == null || !_settings.DataViewList.Any())
                {
                    _logger?.LogError($"{nameof(_settings.DataViewList)} is null or empty. Please provide valid data for training.");
                    throw new InvalidOperationException($"{nameof(_settings.DataViewList)} is null or empty. Please provide valid data for training.");
                }

                _logger?.LogInformation($"Using data view list with {_settings.DataViewList.Count()} items for training.");
                PredictionEngine.DataViewList = _settings.DataViewList;
            }

            if (File.Exists(_settings.AIModelFilePath))
            {
                _logger?.LogInformation($"Model file already exists at {_settings.AIModelFilePath}. Deleting existing model file...");
                File.Delete(_settings.AIModelFilePath);
            }

            PredictionEngine.Logger = _logger;
            PredictionEngine.StopWords = _settings.StopWords;
            PredictionEngine.TextFeaturizingEstimatorOptions = _settings.TextFeaturizingEstimatorOptions;
            PredictionEngine.SdcaMaximumEntropyOptions = _settings.SdcaMaximumEntropyOptions;
            PredictionEngine.ExtendedColumnNames = _settings.ExtendedColumnNames;

            _logger?.LogInformation("Creating model...");
            await PredictionEngine.CreateModelAsync<TModelInput>(_settings.AIModelFilePath);

            var modelExists = File.Exists(_settings.AIModelFilePath);
            if (modelExists)
            {
                _logger?.LogInformation($"Model created successfully at {_settings.AIModelFilePath}.");
            }
            else
            {
                _logger?.LogError("Model creation failed. Model file does not exist after creation attempt.");
            }
            return modelExists;
        }
    }
}
