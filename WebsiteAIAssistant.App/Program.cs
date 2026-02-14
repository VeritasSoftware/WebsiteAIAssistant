// See https://aka.ms/new-console-template for more information
using WebsiteAIAssistant.App;
using WebsiteAIAssistant.PredictionEngine;

Console.WriteLine("Hello, AI using ML .NET!");

PredictionEngine.DataViewType = DataViewType.Text;
PredictionEngine.DataViewPath = "TrainingDataset.tsv";

//Create the model only once as a .zip file is created and can be reused for subsequent predictions
await TestExecutor.CreateModelAsync();

//Load the .zip model file and create the prediction engine
await TestExecutor.CreatePredictionEngineAsync();

//ACCU
await TestExecutor.ExecuteAsync("What are the requisites for carbon credits?");

//ACCU
await TestExecutor.ExecuteAsync("how do I change my business techniques to enhance efficiency and lower emissions?");

await TestExecutor.ExecuteAsync("how do I alter my establishment's processes to improve effectiveness and reduce emissions?");

//NGER
await TestExecutor.ExecuteAsync("I want to report on my emissions");

//NGER
await TestExecutor.ExecuteAsync("what is my mandatory emission reporting?");

//NGER
await TestExecutor.ExecuteAsync("how do I calculate net emissions?");

//Safeguard Mechanism
await TestExecutor.ExecuteAsync("how are emission baselines calculated?");

//Invalid search
await TestExecutor.ExecuteAsync("987 abc");

//Invalid search
await TestExecutor.ExecuteAsync("xyz abc pqr");

//Invalid search
await TestExecutor.ExecuteAsync("what is the colour of a rose?");

Console.WriteLine("=============== End of process, hit any key to finish ===============");
Console.ReadKey();
