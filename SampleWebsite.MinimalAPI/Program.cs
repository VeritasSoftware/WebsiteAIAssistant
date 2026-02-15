using Microsoft.OpenApi;
using SampleWebsite.MinimalAPI;
using WebsiteAIAssistant.MinimalAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Website AI Assistant
builder.Services.AddRouting();
//Optional: register a custom post-prediction service to handle the prediction results
//builder.Services.AddScoped<IPostPredictionService, PostPredictionService>();
builder.Services.AddWebsiteAIAssistant(settings =>
{
    // Path to load model
    string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");
    settings.AIModelFilePath = modelPath;

    settings.NegativeConfidenceThreshold = 0.70f;
    settings.NegativeLabel = -1f;
});

//Swagger
builder.Services.AddEndpointsApiExplorer(); // Required for Minimal APIs
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sample Website AI Assistant Minimal API",
        Description = "AI Assistant helps visitors to your website, narrow down which of the offered products or services suits their needs.",
        Version = "v1"
    });
});

builder.Services.AddMvc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample Website AI Assistant Minimal API V1");
        // Optional: set the UI to load at the app root URL
        // c.RoutePrefix = string.Empty; 
    });
}

app.UseHttpsRedirection();

//Website AI Assistant
app.UseRouting();
app.MapWebsiteAIAssistant();

app.Run();