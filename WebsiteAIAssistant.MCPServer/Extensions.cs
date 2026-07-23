using Microsoft.Extensions.DependencyInjection;
using System;

namespace WebsiteAIAssistant.MCPServer
{
    public static class Extensions
    {
        public static IServiceCollection AddWebsiteAIAssistantMCPServer(this IServiceCollection services, 
                                                                        Action<WebsiteAIAssistantSettings> settings)
        {
            services.AddWebsiteAIAssistantCore(settings);            

            services.AddHostedService<AIModelLoader>();

            services
                .AddMcpServer()
                .WithStdioServerTransport()
                .WithToolsFromAssembly();

            return services;
        }
    }
}
