using Microsoft.AspNetCore.Mvc;
namespace WebsiteAIAssistant.MinimalAPI
{
    public interface IPostPredictionService
    {
        Task<object> HandlePredictionAsync(HttpRequest request, ModelInput input, Prediction prediction);
    }
}
