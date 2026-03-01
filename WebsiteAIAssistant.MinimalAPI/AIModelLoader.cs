namespace WebsiteAIAssistant.MinimalAPI
{
    internal class AIModelLoader : IHostedService
    {
        private readonly WebsiteAIAssistantSettings _settings;

        public AIModelLoader(WebsiteAIAssistantSettings settings)
        {
            _settings = settings;
        }
        private async Task LoadModelAsync()
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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await LoadModelAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
