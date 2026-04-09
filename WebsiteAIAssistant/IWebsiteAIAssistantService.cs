using System;
using System.Threading.Tasks;

namespace WebsiteAIAssistant
{
    public interface IWebsiteAIAssistantService
    {
        bool IsPredictionEngineInitialized { get; }
        [Obsolete("LoadModelAsync is obsolete. The model is now loaded automatically when the PredictionEnginePool is accessed. It will be removed in a future version.")]
        Task<bool> LoadModelAsync();
        Task<bool> UnloadModelAsync();
        Task<Prediction> PredictAsync(ModelInput modelInput);
    }
}