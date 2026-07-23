# Website AI Assistant MCP Server

This is a `Model Context Protocol (MCP) Server` for a Website AI Assistant. It provides an tool for generating responses based on user's input.

The response can be a Prediction or your own custom response (eg. data from database or other source) based on the Prediction.

|Packages|Version|Downloads|
|---------------------------|:---:|:---:|
|*WebsiteAIAssistant*|[![Nuget Version](https://img.shields.io/nuget/v/WebsiteAIAssistant)](https://www.nuget.org/packages/WebsiteAIAssistant)|[![Downloads count](https://img.shields.io/nuget/dt/WebsiteAIAssistant)](https://www.nuget.org/packages/WebsiteAIAssistant)|

## Response

The Server returns a `Prediction` by default.

But, you can also implement a `Post Prediction Service`, in which you can return any response you want.

Eg. you can return information about the predicted category from database or other source.

Just implement the `IPostPredictionService` interface and register it in the DI container as `Scoped`.

```csharp
public interface IPostPredictionService
{
    Task<object> HandlePredictionAsync(ModelInput input, Prediction prediction);
}
```

For example, you can return a `Response` object with database results (for eg.) or just a string message.

### Sample

A Client talking to the MCP Server and getting response.

![Client Server interaction](/Docs/MCPServer.png)

## Integration

You [create your model](/Docs/README.md) and save it as a .zip file, and then just provide the path to load the model in the MCP Server settings.

Add the Nuget package or a reference to the `WebsiteAIAssistant.MCPServer` project in your ASP.NET Core application.

Then, in your `Program.cs`, add the following lines to register the MCP Server:

```csharp
//Website AI Assistant
//Optional: register a custom post-prediction service to handle the prediction results
builder.Services.AddScoped<IPostPredictionService, PostPredictionService>();
//Optional: register a custom logger to log the assistant's operations
builder.Services.AddSingleton<IWebsiteAIAssistantLogger, WebsiteAIAssistantLogger>();
builder.Services.AddWebsiteAIAssistantMCPServer(settings =>
{
    // Path to load model
    string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");
    settings.AIModelLoadFilePath = modelPath;

    settings.NegativeConfidenceThreshold = 0.70f;
    settings.NegativeLabel = -1f;
});
```

### Tests

You can browse the [**Tests**](/WebsiteAIAssistant.IntegrationTests/MCPServerTests.cs) to see how to call the MCP Server.

### Sample Server

You can find a sample MCP Server using the library [**here**](/Sample.WebsiteAIAssistant.MCPServer).

### Sample Client

You can find a sample client [**here**](/Sample.WebsiteAIAssistant.MCPServer.Client).