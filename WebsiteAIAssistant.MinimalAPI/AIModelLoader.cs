namespace WebsiteAIAssistant.MinimalAPI
{
    internal class AIModelLoader : IHostedService
    {
        private readonly WebsiteAIAssistantSettings _options;

        public AIModelLoader(WebsiteAIAssistantSettings options)
        {
            _options = options;
        }
        private async Task LoadModelAsync()
        {
            PredictionEngine.NegativeConfidenceThreshold = _options.NegativeConfidenceThreshold;
            PredictionEngine.NegativeLabel = _options.NegativeLabel;
            await PredictionEngine.LoadModelAsync(_options.AIModelFilePath);
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
