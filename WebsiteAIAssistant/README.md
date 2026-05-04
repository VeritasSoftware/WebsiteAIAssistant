# Website AI Assistant

## Library built using ML .NET, Microsoft's machine learning platform

## Create your own bespoke AI model and make predictions

AI Assistant helps visitors to your website, narrow down which of the offered products or services suits their needs.

|Packages|Version|Downloads|
|---------------------------|:---:|:---:|
|*WebsiteAIAssistant.MinimalAPI*|[![Nuget Version](https://img.shields.io/nuget/v/WebsiteAIAssistant.MinimalAPI)](https://www.nuget.org/packages/WebsiteAIAssistant.MinimalAPI)|[![Downloads count](https://img.shields.io/nuget/dt/WebsiteAIAssistant.MinimalAPI)](https://www.nuget.org/packages/WebsiteAIAssistant.MinimalAPI)|
|*WebsiteAIAssistant.AzureFunction*|[![Nuget Version](https://img.shields.io/nuget/v/WebsiteAIAssistant.AzureFunction)](https://www.nuget.org/packages/WebsiteAIAssistant.AzureFunction)|[![Downloads count](https://img.shields.io/nuget/dt/WebsiteAIAssistant.AzureFunction)](https://www.nuget.org/packages/WebsiteAIAssistant.AzureFunction)|
|*WebsiteAIAssistant.AWSLambda*|[![Nuget Version](https://img.shields.io/nuget/v/WebsiteAIAssistant.AWSLambda)](https://www.nuget.org/packages/WebsiteAIAssistant.AWSLambda)|[![Downloads count](https://img.shields.io/nuget/dt/WebsiteAIAssistant.AWSLambda)](https://www.nuget.org/packages/WebsiteAIAssistant.AWSLambda)|

## Overview

Websites usually have sections that contain information about the various products or services offered by the website. 

It may contain a list of the different products or services offered, along with descriptions and more information in other pages.

This AI Assistant can help visitors narrow down which of the website's products or services suits their needs,

by classifying the **visitor's natural language** and/or **numeric based** input into **one of the categories** of products or services offered by the website. 

You can then provide more information about that category.

The library is useful in `text classification scenarios`.

The API provided by the library let you `create your bespoke AI model` based on `your training data` 

and then `load your model` and `make predictions`.

## Documentation

You can solve real-world AI problems using the library API.

### Car Category Classification Model

In this scenario, the classification into a category is done based on **text & numeric training data**.

#### The Car categories

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

#### The training data

It comprises of 2 tab-separated columns.

First is the Label (ie category) column & second is the Feature (text) column.

You can add additional feature columns too.

```
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

The bespoke AI model is built using this training data using API provided by the library.

Then, that model is loaded & predictions are made.

#### The Unit tests on the model

An out of the box, ready to use helper service is provided to load the model & make predictions.

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

Read more [**here**](https://github.com/VeritasSoftware/WebsiteAIAssistant)