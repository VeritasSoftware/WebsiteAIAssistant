namespace WebsiteAIAssistant.AWSLambda
{
    internal class AIModelLoader : IHostedService
    {
        private readonly IWebsiteAIAssistantService _aiAssistantService;

        public AIModelLoader(IWebsiteAIAssistantService aiAssistantService)
        {
            _aiAssistantService = aiAssistantService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _aiAssistantService.LoadModelAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
