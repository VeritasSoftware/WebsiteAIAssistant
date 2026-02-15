namespace WebsiteAIAssistant.MinimalAPI
{
    internal class AIModelLoader : IHostedService
    {
        private readonly WebsiteAIAssistantOptions _options;
        private readonly IServiceProvider _serviceProvider;
        public AIModelLoader(WebsiteAIAssistantOptions options, IServiceProvider serviceProvider)
        {
            _options = options;
            _serviceProvider = serviceProvider;
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
            await Task.CompletedTask; ;
        }
    }
}
