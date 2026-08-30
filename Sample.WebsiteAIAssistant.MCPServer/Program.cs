using WebsiteAIAssistant.MCPServer;

Console.WriteLine("Website AI Assistant MCP Server...");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebsiteAIAssistantMCPServer(settings =>
{
    // Path to load model
    string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");
    settings.AIModelLoadFilePath = modelPath;

    settings.NegativeConfidenceThreshold = 0.70f;
    settings.NegativeLabel = -1f;
});

var app = builder.Build();

app.UseWebsiteAIAssistantMCPServer();
        
await app.RunAsync();
