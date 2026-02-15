# Website AI Assistant

## AI model built using ML .NET, Microsoft's machine learning platform

AI Assistant helps visitors to your website, narrow down which of the offered products or services suits their needs.

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

![Positive](/Docs/ClassDiagram.png)

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

**Step 1** :

Set the data view type and path to the training dataset or the List of ModelInput. The List can come from a database or any other source. 

The training dataset is used to train the model and create the .zip file in Step 2.

```csharp
PredictionEngine.DataViewType = DataViewType.File;
PredictionEngine.DataViewFilePath = "TrainingDataset.tsv";
```

**Step 2** :

Create the model only once and save it as a .zip file, and reuse the file for subsequent predictions.

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