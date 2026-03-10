using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using SampleWebsite.AzureFunction;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
startup.Configure(app);

app.Run();
