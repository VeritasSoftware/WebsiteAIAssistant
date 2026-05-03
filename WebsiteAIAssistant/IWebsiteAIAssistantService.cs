using System.Threading.Tasks;

namespace WebsiteAIAssistant
{
    public interface IWebsiteAIAssistantService
    {
        Task<bool> UnloadModelAsync();
        Task<bool> UnloadModelAsync<TModelInput>()
            where TModelInput : ModelInput;
        Task<Prediction> PredictAsync<TModelInput>(TModelInput modelInput)
            where TModelInput : ModelInput;
    }
}