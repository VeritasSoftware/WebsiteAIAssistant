using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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
                .WithHttpTransport(options =>
                {
                    // Stateless mode is recommended for servers that don't need
                    // server-to-client requests like sampling or elicitation.
                    // See https://csharp.sdk.modelcontextprotocol.io/concepts/transports/transports.html for details.
                    options.Stateless = true;
                })
                .WithTools<WebsiteAIAssistantTools>();

            return services;
        }

        public static WebApplication UseWebsiteAIAssistantMCPServer(this WebApplication app)
        {
            app.MapMcp("mcp");
            app.UseHttpsRedirection();

            return app;
        }
    }
}
