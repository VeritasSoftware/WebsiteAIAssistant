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
            _logger?.LogInformation("Starting model creation process...");
            PredictionEngine.DataViewType = DataViewType.File;

            if (_settings.DataViewType == DataViewType.File)
            {
                if (string.IsNullOrEmpty(_settings.DataViewFilePath))
                {
                    _logger?.LogError($"{nameof(_settings.DataViewFilePath)} is null or empty. Please provide a valid file path to the training data.");
                    throw new InvalidOperationException($"{nameof(_settings.DataViewFilePath)} is null or empty. Please provide a valid file path to the training data.");
                }

                PredictionEngine.DataViewFilePath = _settings.DataViewFilePath;
            }
            else
            {
                if (_settings.DataViewList == null || !_settings.DataViewList.Any())
                {
                    _logger?.LogError($"{nameof(_settings.DataViewList)} is null or empty. Please provide valid data for training.");
                    throw new InvalidOperationException($"{nameof(_settings.DataViewList)} is null or empty. Please provide valid data for training.");
                }

                PredictionEngine.DataViewList = _settings.DataViewList;
            }

            if (File.Exists(_settings.AIModelFilePath))
            {
                _logger?.LogInformation($"Model file already exists at {_settings.AIModelFilePath}. Deleting existing model file...");
                File.Delete(_settings.AIModelFilePath);
            }

            _logger?.LogInformation("Creating model...");
            await PredictionEngine.CreateModelAsync(_settings.AIModelFilePath);

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
