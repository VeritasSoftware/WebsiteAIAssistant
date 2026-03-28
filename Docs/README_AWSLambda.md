# Website AI Assistant AWS Lambda

This is an AWS Lambda for a website AI assistant. It provides an endpoint for generating responses based on visitor's input.

The response can be a Prediction or your own custom response (eg. data from database or other source) based on the Prediction.

|Packages|Version|Downloads|
|---------------------------|:---:|:---:|
|*WebsiteAIAssistant*|[![Nuget Version](https://img.shields.io/nuget/v/WebsiteAIAssistant)](https://www.nuget.org/packages/WebsiteAIAssistant)|[![Downloads count](https://img.shields.io/nuget/dt/WebsiteAIAssistant)](https://www.nuget.org/packages/WebsiteAIAssistant)|
|*WebsiteAIAssistant.AWSLambda*|[![Nuget Version](https://img.shields.io/nuget/v/WebsiteAIAssistant.AWSLambda)](https://www.nuget.org/packages/WebsiteAIAssistant.AWSLambda)|[![Downloads count](https://img.shields.io/nuget/dt/WebsiteAIAssistant.AWSLambda)](https://www.nuget.org/packages/WebsiteAIAssistant.AWSLambda)|

## Endpoint

Out of the box, the Function provides a single endpoint:

GET /ai/{input}

## Response

The Lambda returns a `Prediction` by default.

![Prediction response](/Docs/LambdaPredictionResponse.png)

But, you can also implement a `Post Prediction Service`, in which you can return any response you want.

Eg. you can return information about the predicted category from database or other source.

Just implement the `IPostPredictionService` interface and register it in the DI container as `Scoped`.

```csharp

public interface IPostPredictionService
{
    Task<object> HandlePredictionAsync(ILambdaContext context, ModelInput input, Prediction prediction);
}
```

## Negative

![Negative](/Docs/NegativeLambda.png)

## Integration

You [create your model](/Docs/README.md) and save it as a .zip file, and then just provide the path to load the model in the Lambda settings.

Add the Nuget package or a reference to the `WebsiteAIAssistant.AWSLambda` project in your ASP.NET Core AWS Lambda project.

Then, in your `Program.cs`, add the following lines to register the Lambda:

```csharp
//Website AI Assistant
//Optional: register a custom post-prediction service to handle the prediction results
//builder.Services.AddScoped<IPostPredictionService, PostPredictionService>();
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