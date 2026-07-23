using Microsoft.Extensions.Hosting;
using WebsiteAIAssistant.MCPServer;

Console.WriteLine("Website AI Assistant MCP Server...");

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWebsiteAIAssistantMCPServer(settings =>
{
    // Path to load model
    string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");
    settings.AIModelLoadFilePath = modelPath;

    settings.NegativeConfidenceThreshold = 0.70f;
    settings.NegativeLabel = -1f;
});

await builder.Build().RunAsync();
