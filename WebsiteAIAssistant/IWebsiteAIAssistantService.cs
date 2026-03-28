using System.Threading.Tasks;

namespace WebsiteAIAssistant
{
    public interface IWebsiteAIAssistantService
    {
        Task<bool> LoadModelAsync();
        Task<bool> UnloadModelAsync();
        Task<Prediction> PredictAsync(ModelInput modelInput);
    }
}