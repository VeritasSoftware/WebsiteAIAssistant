# Website AI Assistant

## Library built using ML .NET, Microsoft's machine learning platform

## Create your own bespoke AI model and make predictions

AI Assistant helps visitors to your website, narrow down which of the offered products or services suits their needs.

|Packages|Version|Downloads|
|---------------------------|:---:|:---:|
|*WebsiteAIAssistant*|[![Nuget Version](https://img.shields.io/nuget/v/WebsiteAIAssistant)](https://www.nuget.org/packages/WebsiteAIAssistant)|[![Downloads count](https://img.shields.io/nuget/dt/WebsiteAIAssistant)](https://www.nuget.org/packages/WebsiteAIAssistant)|
|*WebsiteAIAssistant.MinimalAPI*|[![Nuget Version](https://img.shields.io/nuget/v/WebsiteAIAssistant.MinimalAPI)](https://www.nuget.org/packages/WebsiteAIAssistant.MinimalAPI)|[![Downloads count](https://img.shields.io/nuget/dt/WebsiteAIAssistant.MinimalAPI)](https://www.nuget.org/packages/WebsiteAIAssistant.MinimalAPI)|
|*WebsiteAIAssistant.AzureFunction*|[![Nuget Version](https://img.shields.io/nuget/v/WebsiteAIAssistant.AzureFunction)](https://www.nuget.org/packages/WebsiteAIAssistant.AzureFunction)|[![Downloads count](https://img.shields.io/nuget/dt/WebsiteAIAssistant.AzureFunction)](https://www.nuget.org/packages/WebsiteAIAssistant.AzureFunction)|
|*WebsiteAIAssistant.AWSLambda*|[![Nuget Version](https://img.shields.io/nuget/v/WebsiteAIAssistant.AWSLambda)](https://www.nuget.org/packages/WebsiteAIAssistant.AWSLambda)|[![Downloads count](https://img.shields.io/nuget/dt/WebsiteAIAssistant.AWSLambda)](https://www.nuget.org/packages/WebsiteAIAssistant.AWSLambda)|

[![Build & Test](https://github.com/VeritasSoftware/WebsiteAIAssistant/actions/workflows/dotnet.yml/badge.svg)](https://github.com/VeritasSoftware/WebsiteAIAssistant/actions/workflows/dotnet.yml)

## Overview

Websites usually have sections that contain information about the various products or services offered by the website. 

It may contain a list of the different products or services offered, along with descriptions and more information in other pages.

This AI Assistant can help visitors narrow down which of the website's products or services suits their needs,

by classifying the **visitor's natural language** and/or **numeric based** input into **one of the categories** of products or services offered by the website.

You can then provide more information about that category.

The library is useful in `text classification scenarios`.

The API provided by the library let you `create your bespoke AI model` based on `your training data` 

and then `load your model` and `make predictions`.

## Example

This example is a scenario involving classifying user's natural language input to a predicted category.

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

### WebsiteAIAssistantCreateModelService

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

### WebsiteAIAssistantService

Below is the class diagram of the `WebsiteAIAssistantService` service.

![WebsiteAIAssistant Service](/Docs/WebsiteAIAssistantService.png)

The `WebsiteAIAssistantService` service can be used to load the model and make predictions.

**Note:-** If you do not explicitly load the model, the service will automatically load the model (from the specified path in the Settings) when the first prediction is made.

**Note:-** The `WebsiteAIAssistantService` service uses the [`PredictionEnginePool`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.ml.predictionenginepool-2.-ctor?view=ml-dotnet-preview) to manage the prediction engine instances, 

which allows for better performance and scalability by reusing the prediction engine instances across multiple predictions.

It is also thread-safe, so it can be used in a multi-threaded environment without any issues.

The `WebsiteAIAssistantService` service & the `WebsiteAIAssistantSettings` have to be wired up for dependency injection as shown below.

```csharp
services.AddWebsiteAIAssistantCore(settings =>
{
    settings.AIModelLoadFilePath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");
    settings.NegativeConfidenceThreshold = 0.70f;
    settings.NegativeLabel = -1f;
});
```

If you want to use the generic version of the `WebsiteAIAssistantService`, to support multiple feature columns,

then you can wire it up as shown below.

```csharp
services.AddWebsiteAIAssistantCore<ModelInputExtended>(settings =>
{
    settings.AIModelLoadFilePath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");
    settings.NegativeConfidenceThreshold = 0.70f;
    settings.NegativeLabel = -1f;
});
```

## Usage

If the AI model is trained using a text file, the file must be in a tab-separated format with two columns and no header (by default). 

If you want a header row, you must set the `TrainingDatasetTextFileHasHeader` property of the `PredictionEngine` or of the `WebsiteAIAssistantCreateModelSettings` to true.

First column contains the category Label and the other column (Feature) contains the training text data.

In the example, the training text data is all public information on the website about the various Schemes offered by the website.

Eg. ACCU Scheme has Label 0 and the training text data contains all the information about the scheme.

You can take a look at the training dataset [**here**](/WebsiteAIAssistant.App/TrainingDataset.tsv)

Please note that the training dataset is very small and is only for demonstration purposes.

**Note:-** Create an entry for `NegativeLabel` (eg. -1 default Label, with no Feature data) in the training dataset for testing the negative case where the visitor's input does not match any of the categories of products or services offered by the website.

## Using the Prediction Engine

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

This `PredictAsync` is not thread-safe. 

You should use the helper service `WebsiteAIAssistantService` for making predictions in a multi-threaded environment.

```csharp
var input = new ModelInput { Feature = "What are the requisites for carbon credits?" };

var prediction = await PredictionEngine.PredictAsync(input);
```

### Using the Helper Services

First wire up the services for dependency injection as shown in the previous section.

**Step 2** :

Create the model only once and save it as a `.zip` file, and reuse the file for subsequent predictions.

```csharp
var createModelService = _serviceProvider.GetRequiredService<IWebsiteAIAssistantCreateModelService>();

await createModelService.CreateModelAsync();
```

**Step 3** : 

Test the prediction engine with a sample input.

```csharp
var aiAssistantService = _serviceProvider.GetRequiredService<IWebsiteAIAssistantService>();

var input = new ModelInput { Feature = "What are the requisites for carbon credits?" };

var prediction = await aiAssistantService.PredictAsync(input);
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

### Tests

You can browse the [**Tests**](/WebsiteAIAssistant.Tests/WebsiteAIAssistantTests.cs) to see how to use the library.

### Sample

You can find 

* the training dataset used in the samples [**here**](/WebsiteAIAssistant.Tests/Data/TrainingDataset.tsv).

* a sample Console App using the library [**here**](/WebsiteAIAssistant.App).

* a sample ASP .NET Core Minimal API using the library [**here**](/SampleWebsite.MinimalAPI).

* a sample Azure Function using the library [**here**](/SampleWebsite.AzureFunction).

* a sample AWS Lambda using the library [**here**](/SampleWebsite.AWSLambda).

#### Car Category Classification Model

In this scenario, the classification into a category is done based on **numeric training data** too.

##### The Car categories

```csharp
public enum CarCategory
{
    None = -1,
    TwoDoorBasic = 0,
    TwoDoorLuxury = 1,               
    FourDoorBasic = 2,
    FourDoorLuxury = 3
}
```

##### The training data

```
-1	
0	2 door
0	basic
0	low price $ 20,000
0	mid price $ 25,000
0	high price $ 30,000
1	2 door
1	luxury
1	low price $ 40,000
1	mid price $ 45,000
1	high price $ 50,000
2	4 door
2	basic
2	low price $ 60,000
2	mid price $ 65,000
2	high price $ 70,000
3	4 door
3	luxury
3	low price $ 80,000
3	mid price $ 85,000
3	high price $ 90,000
```

##### The Unit tests on the model

You can see the inputs to the model along with the predicted category.

```csharp
[Theory]
[InlineData("price $ 42,000", CarCategory.TwoDoorLuxury)]
[InlineData("price $ 39,000", CarCategory.TwoDoorBasic)]
[InlineData("price $ 53,000", CarCategory.TwoDoorLuxury)]
[InlineData("4 door price $ 67,000", CarCategory.FourDoorBasic)]
[InlineData("luxury price $ 88,000", CarCategory.FourDoorLuxury)]
[InlineData("luxury price $ 62,000", CarCategory.TwoDoorLuxury)]
[InlineData("2 door price $ 29,000", CarCategory.TwoDoorBasic)]
[InlineData("low price $ 55,000", CarCategory.TwoDoorLuxury)]
[InlineData("high price $ 34,000", CarCategory.TwoDoorBasic)]
[InlineData("What is the colour of a rose?", CarCategory.None)]
public async Task Load_Predict_Service_CarCategory(string userInput, CarCategory expectedResult)
{
    // Arrange                      
    var aiAssistantService = _aiAssistantServiceProvider!.GetRequiredService<IWebsiteAIAssistantService>();

    var input = new ModelInput { Feature = userInput };

    // Act
    var prediction = await aiAssistantService.PredictAsync(input);

    // Assert
    Assert.NotNull(prediction);
    Assert.Equal(expectedResult, (CarCategory)prediction.PredictedLabel);
}
```

You can find the tests of a sample car category classification model created using the library [**here**](/WebsiteAIAssistant.Tests/CarCategoryTests.cs).

The training dataset used for this model [**here**](/WebsiteAIAssistant.Tests/Data/TrainingDataset-CarCategory.tsv).

## Using multiple feature columns in the training dataset

By default, the library supports only one feature column in the training dataset , which is used for training the model.

But, you can add more feature columns by deriving from the `ModelInput` class and adding the additional feature properties.

Then, you can set the `ExtendedFeatureColumnNames` property of the `PredictionEngine` or of the `WebsiteAIAssistantCreateModelSettings`,

to specify the names of the feature columns in the training dataset.

Your training dataset should have the additional feature columns in the same order as specified in the `ExtendedFeatureColumnNames` property.

Below is an example of a training dataset with 3 additional feature columns (Doors, Class, Price).

```csharp
Label Doors	Class	Range	Price
-1	
0	2 door	basic	low	$ 20,000
0	2 door	basic	mid	$ 25,000
0	2 door	basic	high	$ 30,000
1	2 door	luxury	low	$ 40,000
1	2 door	luxury	mid	$ 45,000
1	2 door	luxury	high	$ 50,000
2	4 door	basic	low	$ 60,000
2	4 door	basic	mid	$ 65,000
2	4 door	basic	high	$ 70,000
3	4 door	luxury	low	$ 80,000
3	4 door	luxury	mid	$ 85,000
3	4 door	luxury	high	$ 90,000
```

The derived `ModelInputExtended` class with the additional feature properties can be like shown below.

```csharp
public class ModelInputExtended : ModelInput
{
    [LoadColumn(2)]
    public string Class { get; set; } = string.Empty;
    [LoadColumn(3)]
    public string Range { get; set; } = string.Empty;
    [LoadColumn(4)]
    public string Price { get; set; } = string.Empty;
}
```

The `ExtendedFeatureColumnNames` property can be set as shown below.

```csharp
// Additional configuration for multiple feature columns
PredictionEngine.ExtendedFeatureColumnNames = new[] { $"{nameof(ModelInputExtended.Class)}",
                                                    $"{nameof(ModelInputExtended.Range)}",
                                                    $"{nameof(ModelInputExtended.Price)}"};
```

You create the model as shown below:

```csharp
await PredictionEngine.CreateModelAsync<ModelInputExtended>(modelPath);
```

##### The Unit tests on the model

For making predictions in a multiple feature columns scenario, you must use the helper service `WebsiteAIAssistantService`.

You can see the inputs to the model along with the predicted category.

```csharp
[Theory]
[InlineData("2 door", "", "", "$ 42,000", CarCategory.TwoDoorLuxury)]
[InlineData("", "luxury", "", "$ 100,000", CarCategory.FourDoorLuxury)]
[InlineData("2 door", "", "", "$ 27,000", CarCategory.TwoDoorBasic)]
[InlineData("", "basic", "", "$ 75,000", CarCategory.FourDoorBasic)]
[InlineData("", "", "low", "$ 50,000", CarCategory.TwoDoorLuxury)]
[InlineData("", "", "high", "$ 70,000", CarCategory.FourDoorBasic)]
[InlineData("What is the colour of a rose?", "", "", "", CarCategory.None)]
public async Task Load_Predict_Service_CarCategory_MultipleFeatureColumns(string feature, string feature1, string feature2, string feature3, CarCategory expectedResult)
{
    // Arrange                      
    var aiAssistantService = _aiAssistantServiceProvider!.GetRequiredService<IWebsiteAIAssistantService>();

    var input = new ModelInputExtended 
    { 
        Feature = feature,
        Class = feature1,
        Range = feature2,
        Price = feature3,
    };

    // Act
    var prediction = await aiAssistantService.PredictAsync(input);

    // Assert
    Assert.NotNull(prediction);
    Assert.Equal(expectedResult, (CarCategory)prediction.PredictedLabel);
}
```

The Unit tests for a sample model with multiple feature columns can be found [**here**](/WebsiteAIAssistant.Tests/CarCategoryTests.cs).

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