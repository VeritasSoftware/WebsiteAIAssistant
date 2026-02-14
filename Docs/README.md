# Website AI Assistant

Websites usually have sections that contain information about the various products or services offered by the website. 

It may contain a list of the different products or services offered, along with descriptions and more information in other pages.

This AI Assistant can help visitors narrow down which of the website's products or services suits their needs,

by classifying the **visitor's natural language input** into **one of the categories** of products or services offered by the website, 

You can then provide more information about that category.

For eg. [This](https://cer.gov.au/) web site has **Schemes**, which are a types of products or services offered by the website.

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

ACCU has Label 0 and the training text data contains all the information about ACCU scheme.

You can take a look at the training dataset [**here**](/WebsiteAIAssistant.App/TrainingDataset.tsv)

Please note that the training dataset is very small and is only for demonstration purposes.

Also, create an entry for -1 Label (with no Feature data) in the training dataset for testing the negative case where the visitor's input does not match any of the categories of products or services offered by the website.


```csharp
PredictionEngine.DataViewType = DataViewType.Text;
PredictionEngine.DataViewPath = "TrainingDataset.tsv";

//Create the model only once as a .zip file and reuse the file for subsequent predictions
await TestExecutor.CreateModelAsync();

//Load the .zip model file and create the prediction engine
await TestExecutor.CreatePredictionEngineAsync();

//Test the prediction engine with a sample input

//ACCU is a type of carbon credit scheme, so the expected output is "ACCU"
await TestExecutor.ExecuteAsync("What are the requisites for carbon credits?");
```