// See https://aka.ms/new-console-template for more information
using SampleWebsite.AWSLambda;
using WebsiteAIAssistant;
using WebsiteAIAssistant.AWSLambda;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(config => config.AddConsole());
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

builder.Services.AddHostedService<TestExecutor>(); // Register the test executor as a hosted service to run on startup

var app = builder.Build();

app.Run();