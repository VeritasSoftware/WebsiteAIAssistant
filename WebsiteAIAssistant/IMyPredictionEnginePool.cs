using Microsoft.Extensions.ML;

namespace WebsiteAIAssistant
{
    public interface IMyPredictionEnginePool
    {
        PredictionEnginePool<TModelInput, Prediction> GetPredictionEnginePool<TModelInput>()
            where TModelInput : ModelInput;
    }
}
