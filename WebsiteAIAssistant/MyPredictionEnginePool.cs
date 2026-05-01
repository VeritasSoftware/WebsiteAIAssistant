using Microsoft.Extensions.ML;
using System;

namespace WebsiteAIAssistant
{
    public class MyPredictionEnginePool : IMyPredictionEnginePool
    {
        private readonly IServiceProvider _serviceProvider;
        public MyPredictionEnginePool(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public PredictionEnginePool<TModelInput, Prediction> GetPredictionEnginePool<TModelInput>()
            where TModelInput : ModelInput
        {
            return (PredictionEnginePool<TModelInput, Prediction>)_serviceProvider.GetService(typeof(PredictionEnginePool<TModelInput, Prediction>));
        }
    }
}
