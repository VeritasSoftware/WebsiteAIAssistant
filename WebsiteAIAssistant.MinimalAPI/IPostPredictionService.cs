using Microsoft.AspNetCore.Mvc;
namespace WebsiteAIAssistant.MinimalAPI
{
    public interface IPostPredictionService
    {
        Task<object> HandlePredictionAsync(ModelInput input, Prediction prediction);
    }
}
