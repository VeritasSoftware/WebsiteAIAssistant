using Microsoft.AspNetCore.Http;
namespace WebsiteAIAssistant.AzureFunction
{
    public interface IPostPredictionService
    {
        Task<object> HandlePredictionAsync(HttpRequest request, ModelInput input, Prediction prediction);
    }
}
