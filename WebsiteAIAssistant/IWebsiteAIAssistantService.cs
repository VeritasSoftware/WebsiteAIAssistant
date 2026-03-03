using System.Threading.Tasks;

namespace WebsiteAIAssistant
{
    public interface IWebsiteAIAssistantService
    {
        Task LoadModelAsync();
        Task<Prediction> PredictAsync(ModelInput modelInput);
    }
}