# Website AI Assistant Minimal API

This is a minimal API for a website AI assistant. It provides an endpoint for generating responses based on visitor's input.

## Endpoint

Out of the box, the Minimal API provides a single endpoint:

GET /ai/{input}

![MinimalAPI endpoint](/Docs/MinimalAPIEndpoint.png)

## Response

The API returns a `Prediction` by default.

![Prediction response](/Docs/PredictionResponse.png)

But, you can also implement a `Post Prediction Service`, in which you can return any response you want.

Just implement the `IPostPredictionService` interface and register it in the DI container.

```csharp
public interface IPostPredictionService
{
    Task<object> HandlePredictionAsync(ModelInput input, Prediction prediction);
}
```

For example, you can return a `Response` object with database results (for eg.) or just a string message.

![Post Prediction Service response](/Docs/PostPredictionServiceResponse.png)

## Integration

Add a reference to the `WebsiteAIAssistant.MinimalAPI` project in your ASP.NET Core application.

Then, in your `Program.cs`, add the following lines to register the minimal API:

```csharp
//Website AI Assistant
builder.Services.AddRouting();
//Optional: register a custom post-prediction service to handle the prediction results
builder.Services.AddScoped<IPostPredictionService, PostPredictionService>();
builder.Services.AddWebsiteAIAssistant(options =>
{
    // Path to load model
    string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");
    options.AIModelFilePath = modelPath;

    options.NegativeConfidenceThreshold = 0.70f;
    options.NegativeLabel = -1f;
});
```

Then, add the following lines to map the minimal API endpoints:

```csharp
//Website AI Assistant
app.UseRouting();
app.MapWebsiteAIAssistant();
```