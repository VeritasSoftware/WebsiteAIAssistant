namespace WebsiteAIAssistant.MinimalAPI
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
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
