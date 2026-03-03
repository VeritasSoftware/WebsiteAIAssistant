using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteAIAssistant
{
    public class WebsiteAIAssistantCreateModelService : IWebsiteAIAssistantCreateModelService
    {
        private readonly WebsiteAIAssistantCreateModelSettings _settings;

        public WebsiteAIAssistantCreateModelService(WebsiteAIAssistantCreateModelSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<bool> CreateModelAsync()
        {
            PredictionEngine.DataViewType = DataViewType.File;

            if (_settings.DataViewType == DataViewType.File)
            {
                if (string.IsNullOrEmpty(_settings.DataViewFilePath))
                {
                    throw new InvalidOperationException("DataViewFilePath is null or empty. Please provide a valid file path to the training data.");
                }

                PredictionEngine.DataViewFilePath = _settings.DataViewFilePath;
            }
            else
            {
                if (_settings.DataViewList == null || !_settings.DataViewList.Any())
                {
                    throw new InvalidOperationException("DataViewList is null or empty. Please provide valid data for training.");
                }

                PredictionEngine.DataViewList = _settings.DataViewList;
            }

            if (File.Exists(_settings.AIModelFilePath))
            {
                File.Delete(_settings.AIModelFilePath);
            }

            await PredictionEngine.CreateModelAsync(_settings.AIModelFilePath);

            var modelExists = File.Exists(_settings.AIModelFilePath);
            return modelExists;
        }
    }
}
