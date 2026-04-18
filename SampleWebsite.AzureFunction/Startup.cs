using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebsiteAIAssistant;
using WebsiteAIAssistant.AzureFunction;

namespace SampleWebsite.AzureFunction
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(config => config.AddConsole());
            //Website AI Assistant            
            //Optional: register a custom post-prediction service to handle the prediction results
            //services.AddScoped<IPostPredictionService, PostPredictionService>();
            services.AddSingleton<IWebsiteAIAssistantLogger, WebsiteAIAssistantLogger>();
            services.AddWebsiteAIAssistant(settings =>
            {
                // Path to load model
                string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");
                settings.AIModelLoadFilePath = modelPath;

                settings.NegativeConfidenceThreshold = 0.70f;
                settings.NegativeLabel = "-1";
            });         
        }

        //public void Configure(IApplicationBuilder app, WebApplication webApplication)
        public void Configure(IHost app, IWebHostEnvironment? environment = null)
        {                        
        }
    }
}
