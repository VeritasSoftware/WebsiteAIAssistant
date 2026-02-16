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
