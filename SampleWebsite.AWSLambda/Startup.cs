using WebsiteAIAssistant.AWSLambda;

namespace SampleWebsite.AWSLambda
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
            services.AddWebsiteAIAssistant(settings =>
            {
                // Path to load model
                string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");
                settings.AIModelLoadFilePath = modelPath;

                settings.NegativeConfidenceThreshold = 0.70f;
                settings.NegativeLabel = -1f;
            });
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
