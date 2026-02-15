using System.Runtime.InteropServices;
using WebsiteAIAssistant;
using WebsiteAIAssistant.MinimalAPI;

namespace SampleWebsite.MinimalAPI
{
    public class PostPredictionService : IPostPredictionService
    {
        public async Task<object> HandlePredictionAsync(ModelInput input, Prediction prediction)
        {
            // Here you can process the prediction result as needed. For demonstration, we will just return a string.
            // You can return any object that makes sense for your application, such as a custom response model or a simple message.
            return await Task.FromResult("Prediction received: " + prediction.PredictedLabel.ToString() 
                                            + " with score: " + string.Join(", ", prediction.Score));
        }
    }
}
