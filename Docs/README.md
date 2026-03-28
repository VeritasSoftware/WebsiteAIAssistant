# Website AI Assistant

## AI model built using ML .NET, Microsoft's machine learning platform

AI Assistant helps visitors to your website, narrow down which of the offered products or services suits their needs.

|Packages|Version|Downloads|
|---------------------------|:---:|:---:|
|*WebsiteAIAssistant*|[![Nuget Version](https://img.shields.io/nuget/v/WebsiteAIAssistant)](https://www.nuget.org/packages/WebsiteAIAssistant)|[![Downloads count](https://img.shields.io/nuget/dt/WebsiteAIAssistant)](https://www.nuget.org/packages/WebsiteAIAssistant)|
|*WebsiteAIAssistant.MinimalAPI*|[![Nuget Version](https://img.shields.io/nuget/v/WebsiteAIAssistant.MinimalAPI)](https://www.nuget.org/packages/WebsiteAIAssistant.MinimalAPI)|[![Downloads count](https://img.shields.io/nuget/dt/WebsiteAIAssistant.MinimalAPI)](https://www.nuget.org/packages/WebsiteAIAssistant.MinimalAPI)|

## Overview

Websites usually have sections that contain information about the various products or services offered by the website. 

It may contain a list of the different products or services offered, along with descriptions and more information in other pages.

This AI Assistant can help visitors narrow down which of the website's products or services suits their needs,

by classifying the **visitor's natural language input** into **one of the categories** of products or services offered by the website, 

You can then provide more information about that category.

You can create a Web API with an GET endpoint that takes the visitor's input and returns the predicted category and more information about the category from the website's database or other source.

## Example

Let us say a web site has **Schemes**, which are a types of products or services offered by the website.

The visitor's input is classified into a predicted **Scheme**.

## Positive

![Positive](/Docs/Positive.png)

## Negative

![Negative](/Docs/Negative.png)

## Implementation

Below is the class diagram of the `core` component.

![Positive](/Docs/ClassDiagram.png)

## Helper Services

Below is the class diagram of the `WebsiteAIAssistantCreateModelService` service.

![WebsiteAIAssistant Create Model Service](/Docs/WebsiteAIAssistantCreateModelService.png)

This service (& the `WebsiteAIAssistantCreateModelSettings`) can be wired up for dependency injection as `Singleton`, 

and can be used to create the model and save it as a .zip file.

```csharp
var createModelSettings = new WebsiteAIAssistantCreateModelSettings
{
    DataViewType = DataViewType.File,
    DataViewFilePath = Path.Combine(Environment.CurrentDirectory, "TrainingDataset.tsv"),
    AIModelFilePath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip")
};

// Register services for dependency injection
services.AddSingleton(createModelSettings);
services.AddSingleton<IWebsiteAIAssistantCreateModelService, WebsiteAIAssistantCreateModelService>();
```

Below is the class diagram of the `WebsiteAIAssistantService` service.

![WebsiteAIAssistant Service](/Docs/WebsiteAIAssistantService.png)

This service (& the `WebsiteAIAssistantSettings`) can be wired up for dependency injection as `Singleton`, 

and can be used to load the model and make predictions.

If you do not explicitly load the model, the service will automatically load the model (from the specified path in the Settings) when the first prediction is made.


```csharp
var settings = new WebsiteAIAssistantSettings
{                
    AIModelFilePath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip"),
    NegativeConfidenceThreshold = 0.70f,
    NegativeLabel = -1f
};

// Register services for dependency injection
services.AddSingleton(settings);
services.AddSingleton<IWebsiteAIAssistantService, WebsiteAIAssistantService>();
```

## Usage

If the AI model is trained using a text file, the file must be in a tab-separated format with two columns and no header.

First column contains the category Label and the other column (Feature) contains the training text data.

In the example, the training text data is all public information on the website about the various Schemes offered by the website.

Eg. ACCU Scheme has Label 0 and the training text data contains all the information about the scheme.

You can take a look at the training dataset [**here**](/WebsiteAIAssistant.App/TrainingDataset.tsv)

Please note that the training dataset is very small and is only for demonstration purposes.

**Note:-** Create an entry for `NegativeLabel` (eg. -1 default Label, with no Feature data) in the training dataset for testing the negative case where the visitor's input does not match any of the categories of products or services offered by the website.

You can set the `NegativeLabel`, to determine the Label for negative predictions. Default value is -1.

```csharp
// Set the negative label (optional)
PredictionEngine.NegativeLabel = -10;
```

You can set the `NegativeConfidenceThreshold` to a value between 0 and 1, to determine the minimum confidence score for a negative prediction to be considered valid. Default value is 0.70.

```csharp
// Set the negative confidence threshold (optional)
PredictionEngine.NegativeConfidenceThreshold = 0.50f;
```

Under the hood, the [`SdcaMaximumEntropyMulticlassTrainer`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.ml.trainers.sdcamaximumentropymulticlasstrainer?view=ml-dotnet-preview) is used to train the model, which is a linear classifier that optimizes the maximum entropy objective function using stochastic dual coordinate ascent (SDCA) algorithm.

You can set the [**options**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.ml.trainers.sdcamaximumentropymulticlasstrainer.options?view=ml-dotnet-preview) for the trainer in the `SdcaMaximumEntropyOptions` property of the `PredictionEngine`.

```csharp
// Set the options for the SdcaMaximumEntropyMulticlassTrainer (optional)
PredictionEngine.SdcaMaximumEntropyOptions = new SdcaMaximumEntropyOptions
{
    BiasLearningRate = 0.1f,
    ConvergenceCheckFrequency = 10,
    MaximumNumberOfIterations = 100,
    ConvergenceTolerance = 0.01f,
    L1Regularization = 0.01f,
    L2Regularization = 0.01f,        
    Shuffle = true
};
```

or you can set the options for the trainer in the `WebsiteAIAssistantCreateModelSettings` when using the `WebsiteAIAssistantCreateModelService`.

Add the Nuget package or a reference to the `WebsiteAIAssistant` project in your ASP.NET Core application.

**Step 1** :

Set the data view type and path to your training dataset or the List of `ModelInput`. The List can come from a database or any other source. 

The training dataset is used to train the model and create the .zip file in Step 2.

```csharp
PredictionEngine.DataViewType = DataViewType.File;
PredictionEngine.DataViewFilePath = "TrainingDataset.tsv";
```

You can also set `SdcaMaximumEntropyOptions` to fine tune the trainer to your needs (as shown earlier).

**Step 2** :

Create the model only once and save it as a `.zip` file, and reuse the file for subsequent predictions.

```csharp
// Path to save model
string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");

await PredictionEngine.CreateModelAsync(modelPath);
```

**Step 3** : 

Load the .zip model file and create the prediction engine.

```csharp
// Path to load model
string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");

await PredictionEngine.LoadModelAsync(modelPath);
```

**Step 4** : 

Test the prediction engine with a sample input.

```csharp
var input = new ModelInput { Feature = "What are the requisites for carbon credits?" };

var prediction = await PredictionEngine.PredictAsync(input);
```

### Logging

If you want to log the model creation, load & predictions, you can implement the `IWebsiteAIAssistantLogger` interface.

Then, pass an instance of your logger implementation to the `PredictionEngine`

```csharp
PredictionEngine.Logger = new YourLoggerImplementation();
```
Or if you are using the helper services, you can register it in the DI container as `Singleton`.

```csharp
services.AddSingleton<IWebsiteAIAssistantLogger, YourLoggerImplementation>();
```

Your logger implementation can be like shown [**here**](/SampleWebsite.MinimalAPI/WebsiteAIAssistantLogger.cs) for ASP .NET Core.

## Website AI Assistant Minimal API

There is a Minimal API endpoint which you can directly use in your API project.

You create your model and save it as a .zip file as shown above, and then just provide the path to load the model in the Minimal API settings.

Read [**more**](/Docs/README_MinimalAPI.md)

GET /ai/{input}

[![MinimalAPI endpoint](/Docs/MinimalAPIEndpoint.png)](/Docs/README_MinimalAPI.md)

## Website AI Assistant Azure Function

There is an Azure Function endpoint which you can directly use in your Functions project.

You create your model and save it as a .zip file as shown above, and then just provide the path to load the model in the settings.

Read [**more**](/Docs/README_AzureFunction.md)

GET /ai/{input}

[![Azure Function endpoint](/Docs/AzureFunctionEndpoint.png)](/Docs/README_AzureFunction.md)

## Website AI Assistant AWS Lambda

There is an AWS Lambda endpoint which you can directly use in your Lambda project.

You create your model and save it as a .zip file as shown above, and then just provide the path to load the model in the settings.

Read [**more**](/Docs/README_AWSLambda.md)

GET /ai/{input}

[![AWS Lambda endpoint](/Docs/AWSLambdaEndpoint.png)](/Docs/README_AWSLambda.md)