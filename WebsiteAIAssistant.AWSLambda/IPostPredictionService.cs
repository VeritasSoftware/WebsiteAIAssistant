using Amazon.Lambda.Core;

namespace WebsiteAIAssistant.AWSLambda
{
    public interface IPostPredictionService
    {
        Task<object> HandlePredictionAsync(ILambdaContext context, ModelInput input, Prediction prediction);
    }
}
