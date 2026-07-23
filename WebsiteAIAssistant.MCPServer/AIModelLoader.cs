using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace WebsiteAIAssistant.MCPServer
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
