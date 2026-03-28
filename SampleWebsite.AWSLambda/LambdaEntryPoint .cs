using Amazon.Lambda.AspNetCoreServer;

namespace SampleWebsite.AWSLambda
{
    public class LambdaEntryPoint : APIGatewayHttpApiV2ProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder
                .UseStartup<Startup>(); // Uses the same Startup as local hosting
        }
    }
}
