using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace WebsiteAIAssistant.AWSLambda
{
    public class WebsiteAIAssistantFunctions
    {
        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public WebsiteAIAssistantFunctions()
        {
        }


        /// <summary>
        /// A Lambda function to respond to HTTP WebsiteAIAssistantGetPrediction methods from API Gateway
        /// </summary>
        /// <remarks>
        /// This uses the <see href="https://github.com/aws/aws-lambda-dotnet/blob/master/Libraries/src/Amazon.Lambda.Annotations/README.md">Lambda Annotations</see> 
        /// programming model to bridge the gap between the Lambda programming model and a more idiomatic .NET model.
        /// 
        /// This automatically handles reading parameters from an APIGatewayProxyRequest
        /// as well as syncing the function definitions to serverless.template each time you build.
        /// 
        /// If you do not wish to use this model and need to manipulate the API Gateway 
        /// objects directly, see the accompanying Readme.md for instructions.
        /// </remarks>
        /// <param name="context">Information about the invocation, function, and execution environment</param>
        /// <returns>The response as an implicit <see cref="APIGatewayProxyResponse"/></returns>
        [LambdaFunction]
        //[HttpApi(LambdaHttpMethod.Get, "/ai")]
        [RestApi(LambdaHttpMethod.Get, "/ai")]
        public IHttpResult Get([FromQuery]string input, ILambdaContext context)
        {
            context.Logger.LogInformation("Handling the 'Get' Request");

            return HttpResults.Ok(input);
        }
    }
}
