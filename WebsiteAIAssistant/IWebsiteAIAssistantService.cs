using System.Threading.Tasks;

namespace WebsiteAIAssistant
{
    public interface IWebsiteAIAssistantService
    {
        bool IsPredictionEngineInitialized { get; }
        Task<bool> UnloadModelAsync();
        Task<Prediction> PredictAsync(ModelInput modelInput);
    }

    public interface IWebsiteAIAssistantService<TModelInput>
        where TModelInput : ModelInput
    {
        bool IsPredictionEngineInitialized { get; }
        Task<bool> UnloadModelAsync();
        Task<Prediction> PredictAsync(TModelInput modelInput);
    }
}