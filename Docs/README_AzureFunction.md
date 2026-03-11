# Website AI Assistant Azure Function

This is an Azure Function for a website AI assistant. It provides an endpoint for generating responses based on visitor's input.

The response can be a Prediction or your own custom response (eg. data from database or other source) based on the Prediction.

|Packages|Version|Downloads|
|---------------------------|:---:|:---:|
|*WebsiteAIAssistant*|[![Nuget Version](https://img.shields.io/nuget/v/WebsiteAIAssistant)](https://www.nuget.org/packages/WebsiteAIAssistant)|[![Downloads count](https://img.shields.io/nuget/dt/WebsiteAIAssistant)](https://www.nuget.org/packages/WebsiteAIAssistant)|
|*WebsiteAIAssistant.AzureFunction*|[![Nuget Version](https://img.shields.io/nuget/v/WebsiteAIAssistant.AzureFunction)](https://www.nuget.org/packages/WebsiteAIAssistant.AzureFunction)|[![Downloads count](https://img.shields.io/nuget/dt/WebsiteAIAssistant.AzureFunction)](https://www.nuget.org/packages/WebsiteAIAssistant.AzureFunction)|

## Endpoint

Out of the box, the Function provides a single endpoint:

GET /ai/{input}

![Azure Function endpoint](/Docs/AzureFunctionEndpoint.png)

## Response

The Function returns a `Prediction` by default.

![Prediction response](/Docs/FunctionPredictionResponse.png)

But, you can also implement a `Post Prediction Service`, in which you can return any response you want.

Eg. you can return information about the predicted category from database or other source.

Just implement the `IPostPredictionService` interface and register it in the DI container as `Scoped`.

```csharp

public interface IPostPredictionService
{
    Task<object> HandlePredictionAsync(HttpRequest request, ModelInput input, Prediction prediction);
}
```

For example, you can return a `Response` object with database results (for eg.) or just a string message.

![Post Prediction Service response](/Docs/FunctionPostPredictionServiceResponse.png)

## Negative

![Negative](/Docs/NegativeFunction.png)

## Integration

You [create your model](/Docs/README.md) and save it as a .zip file, and then just provide the path to load the model in the Function settings.

Add the Nuget package or a reference to the `WebsiteAIAssistant.AzureFunction` project in your ASP.NET Core application.

Then, in your `Program.cs`, add the following lines to register the Function:

```csharp
//Website AI Assistant
//Optional: register a custom post-prediction service to handle the prediction results
builder.Services.AddScoped<IPostPredictionService, PostPredictionService>();
//Optional: register a custom logger to log the assistant's operations
builder.Services.AddSingleton<IWebsiteAIAssistantLogger, WebsiteAIAssistantLogger>();
builder.Services.AddWebsiteAIAssistant(settings =>
{
    // Path to load model
    string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");
    settings.AIModelLoadFilePath = modelPath;

    settings.NegativeConfidenceThreshold = 0.70f;
    settings.NegativeLabel = -1f;
});
```

### Tests

You can browse the [**Tests**](/WebsiteAIAssistant.Tests/AzureFunctionEndpointTests.cs) to see how to call the Function.

### Sample

You can find a sample Function App using the library [**here**](/SampleWebsite.AzureFunction).
