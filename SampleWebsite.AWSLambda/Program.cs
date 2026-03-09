// See https://aka.ms/new-console-template for more information
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using SampleWebsite.AWSLambda;
using WebsiteAIAssistant.AWSLambda;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting();
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();

app.MapControllers(); // or app.UseEndpoints(...)

app.Run(); // Terminal middleware


//public partial class Program
//{
//    public static void Main(string[] args)
//    {
//        CreateHostBuilder(args).Build().Run();
//    }

//    public static IHostBuilder CreateHostBuilder(string[] args) =>
//        Host.CreateDefaultBuilder(args)
//            .ConfigureWebHostDefaults(webBuilder =>
//            {
//                webBuilder.UseStartup<Startup>();
//            });
//}

//Console.WriteLine("WebsiteAIAssistant, AWS Lambda!");

//Environment.SetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME", "", EnvironmentVariableTarget.Process);

//Environment.

//var aiAssistant = new WebsiteAIAssistantFunctions();

//// Create the Lambda handler delegate
//Func<string, ILambdaContext, IHttpResult> handler = aiAssistant.Get;

//// Build the Lambda runtime
//using var handlerWrapper = HandlerWrapper.GetHandlerWrapper(handler, new DefaultLambdaJsonSerializer());
//using var bootstrap = new LambdaBootstrap(handlerWrapper);

//Console.WriteLine("Starting Lambda in console mode...");
//await bootstrap.RunAsync(); // This will keep listening for Lambda events