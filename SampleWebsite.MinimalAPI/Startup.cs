using Microsoft.OpenApi;
using WebsiteAIAssistant.MinimalAPI;

namespace SampleWebsite.MinimalAPI
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
            //Website AI Assistant
            services.AddRouting();
            //Optional: register a custom post-prediction service to handle the prediction results
            //builder.Services.AddScoped<IPostPredictionService, PostPredictionService>();
            services.AddWebsiteAIAssistant(settings =>
            {
                // Path to load model
                string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");
                settings.AIModelFilePath = modelPath;

                settings.NegativeConfidenceThreshold = 0.70f;
                settings.NegativeLabel = -1f;
            });

            //Swagger
            services.AddEndpointsApiExplorer(); // Required for Minimal APIs
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Sample Website AI Assistant Minimal API",
                    Description = "AI Assistant helps visitors to your website, narrow down which of the offered products or services suits their needs.",
                    Version = "v1"
                });
            });

            services.AddMvc();            
        }

        //public void Configure(IApplicationBuilder app, WebApplication webApplication)
        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            // Configure the HTTP request pipeline.
            if (environment.IsDevelopment())
            {
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
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapWebsiteAIAssistant();
            });
        }
    }
}
